using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;

namespace DynamoTests
{
	class Program
	{
		private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
		{
			ServiceURL = "http://localhost:8000"
		});

		static void Main(string[] args)
		{
			string tableName = "TestTable";
			var request = new CreateTableRequest
			{
				AttributeDefinitions = new System.Collections.Generic.List<AttributeDefinition>
				{
					new AttributeDefinition
					{
						AttributeName = "Id",
						AttributeType = "N"
					},
				},
				KeySchema = new System.Collections.Generic.List<KeySchemaElement>
				{
					new KeySchemaElement
					{
						AttributeName = "Id",
						KeyType = "HASH"
					},
				},
				ProvisionedThroughput = new ProvisionedThroughput
				{
					ReadCapacityUnits = 5,
					WriteCapacityUnits = 6
				},
				TableName = tableName,
			};

			var response = client.CreateTableAsync(request).Result;

			var tableDescription = response.TableDescription;
			Console.WriteLine("{1}: {0} \t ReadsPerSec: {2} \t WritesPerSec: {3}",
					  tableDescription.TableStatus,
					  tableDescription.TableName,
					  tableDescription.ProvisionedThroughput.ReadCapacityUnits,
					  tableDescription.ProvisionedThroughput.WriteCapacityUnits);

			string status = tableDescription.TableStatus;
			Console.WriteLine(tableName + " - " + status);
		}

		private static void WaitUntilTableReady(string tableName)
		{
			string status = null;
			// Let us wait until table is created. Call DescribeTable.
			do
			{
				System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
				try
				{
					var res = client.DescribeTableAsync(new DescribeTableRequest
					{
						TableName = tableName
					}).Result;

					Console.WriteLine("Table name: {0}, status: {1}",
							  res.Table.TableName,
							  res.Table.TableStatus);
					status = res.Table.TableStatus;
				}
				catch (ResourceNotFoundException)
				{
					// DescribeTable is eventually consistent. So you might
					// get resource not found. So we handle the potential exception.
				}
			} while (status != "ACTIVE");
		}
	}
}
