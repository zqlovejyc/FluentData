﻿using System.Collections.Generic;
using FluentData.Providers;
using FluentData._Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentData._Documentation
{
	[TestClass]
	public class DocumentationCode : IDbProviderTests
	{
		protected IDbContext GetContext()
		{
			return new DbContext().ConnectionStringName("SqlServer", DbProviderTypes.SqlServer);
		}

		[TestMethod]
		public void Query_many_dynamic()
		{
			var products = GetContext().Sql("select * from Product").Query();

			Assert.IsTrue(products.Count > 0);
		}

		[TestMethod]
		public void Query_single_dynamic()
		{
			var product = GetContext().Sql("select * from Product where ProductId = 1").QuerySingle();

			Assert.IsNotNull(product);
		}

		[TestMethod]
		public void Query_many_strongly_typed()
		{
			var products = GetContext().Sql("select * from Product").Query<Product>();

			Assert.IsTrue(products.Count > 0);
		}

		[TestMethod]
		public void Query_single_strongly_typed()
		{
			var product = GetContext().Sql("select * from Product where ProductId = 1").QuerySingle<Product>();

			Assert.IsNotNull(product);
		}

		[TestMethod]
		public void Query_auto_mapping_alias()
		{
			var product = GetContext().Sql(@"select p.*,
											c.CategoryId as Category_CategoryId,
											c.Name as Category_Name
											from Product p
											inner join Category c on p.CategoryId = c.CategoryId
											where ProductId = 1")
									.QuerySingle<Product>();

			Assert.IsNotNull(product);
			Assert.IsNotNull(product.Category);
			Assert.IsNotNull(product.Category.Name);
		}

		[TestMethod]
		public void Query_custom_mapping_dynamic()
		{
			var products = GetContext().Sql(@"select * from Product")
									.QueryNoAutoMap<Product>(Custom_mapper_using_dynamic);

			Assert.IsNotNull(products[0].Name);
		}

		public void Custom_mapper_using_dynamic(dynamic row, Product product)
		{
			product.ProductId = row.ProductId;
			product.Name = row.Name;
		}

		[TestMethod]
		public void Query_custom_mapping_datareader()
		{
			var products = GetContext().Sql(@"select * from Product")
									.QueryNoAutoMap<Product>(Custom_mapper_using_datareader);

			Assert.IsNotNull(products[0].Name);
		}

		public void Custom_mapper_using_datareader(IDataReader row, Product product)
		{
			product.ProductId = row.GetInt32("ProductId");
			product.Name = row.GetString("Name");
		}

		[TestMethod]
		public void QueryValue()
		{
			int categoryId = GetContext().Sql("select CategoryId from Product where ProductId = 1")
										.QueryValue<int>();

			Assert.AreEqual(1, categoryId);
		}

		[TestMethod]
		public void Unnamed_parameters_one()
		{
			var product = GetContext().Sql("select * from Product where ProductId = @0", 1).QuerySingle();

			Assert.IsNotNull(product);
		}

		[TestMethod]
		public void Unnamed_parameters_many()
		{
			var products = GetContext().Sql("select * from Product where ProductId = @0 or ProductId = @1", 1, 2).Query();

			Assert.AreEqual(2, products.Count);
		}

		[TestMethod]
		public void Named_parameters()
		{
			var products = GetContext().Sql("select * from Product where ProductId = @ProductId1 or ProductId = @ProductId2")
									.Parameter("ProductId1", 1)
									.Parameter("ProductId2", 2)
									.Query();

			Assert.AreEqual(2, products.Count);
		}

		[TestMethod]
		public void In_query()
		{
			var ids = new List<int>() { 1, 2, 3, 4 };

			var products = GetContext().Sql("select * from Product where ProductId in(@0)", ids).Query();

			Assert.AreEqual(4, products.Count);
		}

		[TestMethod]
		public void MultipleResultset()
		{
			using (var command = GetContext().MultiResultSql())
			{
				var categories = command.Sql(@"select * from Category;
												select * from Product;").Query();

				var products = command.Query();

				Assert.IsTrue(categories.Count > 0);
				Assert.IsTrue(products.Count > 0);
			}
		}

		[TestMethod]
		public void Insert_data_sql()
		{
			var productId = GetContext().Sql("insert into Product(Name, CategoryId) values(@0, @1);")
							.Parameters("The Warren Buffet Way", 1)
							.ExecuteReturnLastId();

			Assert.IsTrue(productId > 0);
		}

		[TestMethod]
		public void Insert_data_builder_no_automapping()
		{
			var productId = GetContext().Insert("Product")
								.Column("CategoryId", 1)
								.Column("Name", "The Warren Buffet Way")
								.ExecuteReturnLastId();

			Assert.IsTrue(productId > 0);
		}

		[TestMethod]
		public void Insert_data_builder_automapping()
		{
			var product = new Product();
			product.CategoryId = 1;
			product.Name = "The Warren Buffet Way";

			var productId = GetContext().Insert<Product>("Product", product)
								.IgnoreProperty(x => x.ProductId)
								.AutoMap()
								.ExecuteReturnLastId();

			Assert.IsTrue(productId > 0);
		}

		[TestMethod]
		public void Update_data_sql()
		{
			var rowsAffected = GetContext().Sql("update Product set Name = @0 where ProductId = @1")
								.Parameters("The Warren Buffet Way", 1)
								.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Update_data_builder()
		{
			var rowsAffected = GetContext().Update("Product")
								.Column("Name", "The Warren Buffet Way")
								.Where("ProductId", 1)
								.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Update_data_builder_automapping()
		{
			var product = GetContext().Sql("select * from Product where ProductId = 1")
								.QuerySingle<Product>();
			
			product.Name = "The Warren Buffet Way";

			var rowsAffected = GetContext().Update<Product>("Product", product)
										.Where(x => x.ProductId)
										.AutoMap()
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Delete_data_sql()
		{
			var productId = GetContext().Sql("insert into Product(Name, CategoryId) values(@0, @1);")
							.Parameters("The Warren Buffet Way", 1)
							.ExecuteReturnLastId();

			var rowsAffected = GetContext().Sql("delete from Product where ProductId = @0")
									.Parameters(productId)
									.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Delete_data_builder()
		{
			var productId = GetContext().Sql("insert into Product(Name, CategoryId) values(@0, @1);")
							.Parameters("The Warren Buffet Way", 1)
							.ExecuteReturnLastId();

			var rowsAffected = GetContext().Delete("Product")
										.Where("ProductId", productId)
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Transactions()
		{
			using (var db = GetContext().UseTransaction)
			{
				db.Sql("update Product set Name = @0 where ProductId = @1")
							.Parameters("The Warren Buffet Way", 1)
							.Execute();

				db.Sql("update Product set Name = @0 where ProductId = @1")
							.Parameters("Bill Gates Bio", 2)
							.Execute();

				db.Commit();
			}
		}

		[TestMethod]
		public void Stored_procedure_sql()
		{
			var rowsAffected = GetContext().Sql("execute ProductUpdate @ProductId = @0, @Name = @1")
										.Parameters(1, "The Warren Buffet Way")
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Stored_procedure()
		{
			var rowsAffected = GetContext().Sql("ProductUpdate")
										.CommandType(DbCommandTypes.StoredProcedure)
										.Parameter("ProductId", 1)
										.Parameter("Name", "The Warren Buffet Way")
										.Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void Stored_procedure_builder()
		{
			var rowsAffected = GetContext().StoredProcedure("ProductUpdate")
										.Parameter("Name", "The Warren Buffet Way")
										.Parameter("ProductId", 1).Execute();

			Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void StoredProcedure_builder_automapping()
		{
		    var product = GetContext().Sql("select * from Product where ProductId = 1")
		                    .QuerySingle<Product>();

		    product.Name = "The Warren Buffet Way";

		    var rowsAffected = GetContext().StoredProcedure<Product>("ProductUpdate", product)
											.IgnoreProperty(x => x.CategoryId)
											.AutoMap().Execute();

		    Assert.AreEqual(1, rowsAffected);
		}

		[TestMethod]
		public void StoredProcedure_builder_using_expression()
		{
			var product = GetContext().Sql("select * from Product where ProductId = 1")
							.QuerySingle<Product>();
			product.Name = "The Warren Buffet Way";

			var rowsAffected = GetContext().StoredProcedure<Product>("ProductUpdate", product)
											.Parameter(x => x.ProductId)
											.Parameter(x => x.Name).Execute();

			Assert.AreEqual(1, rowsAffected);
		}
	}
}