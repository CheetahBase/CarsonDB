using System;
using System.Collections.Generic;
using System.Data;
using CarsonDB;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	class Program
	{
		static void Main2(string[] args)
		{
			CarsonConnection connection = new CarsonConnection(@"D:\projects\vetinfo\Hank");
			connection.Open();

			Console.WriteLine("Opened Successfully!!!");

			CarsonCommand command = new CarsonCommand("SELECT * FROM CLIENT;", connection);

			CarsonDataAdapter adapter = new CarsonDataAdapter();
			adapter.SelectCommand = command;

			DataSet dataSet = new DataSet();
			int recordCount = adapter.Fill(dataSet);

			//DateTime startTime = DateTime.Now;
			//List<Client.ClientData> clients = dataSet.Tables[0].ToList<Client.ClientData>();

			//Console.WriteLine("Running Time: " + ((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString());
			Console.ReadLine();
		}

		static void Main3(string[] args)
		{
			Client client = new Client(@"D:\projects\vetinfo\Hank");
			DateTime startTime = DateTime.Now;
			List<Client.ClientData> clientList = client.ClientList();

			Console.WriteLine(clientList.Count);
			Console.WriteLine("Running Time: " + ((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString());
			Console.ReadLine();

			//client.AddFilterCriteria(Client.ClientFields.ClientAdded, ComparisonType.GreaterThan, "1/1/2008");
			//client.AddFilterCriteria(Client.ClientFields.ClientAdded, ComparisonType.LessThan, "1/1/2009");
			//client.ExecuteQuery();
		}

		static void Main4(string[] args)
		{
			DateTime startTime = DateTime.Now;
			Client client = new Client(@"D:\projects\vetinfo\80702");
			client.AddFilterCriteria(Client.ClientFields.ClientAdded, ComparisonType.GreaterThanEqual, new DateTime(1980, 1, 1));
			List<Client.ClientData> clientList = client.ClientList();

			Animal animal = new Animal(@"D:\projects\vetinfo\80702");
			animal.AddFilterCriteria(Animal.AnimalFields.AnimalAdded, ComparisonType.GreaterThanEqual, new DateTime(1980, 1, 1));
			List<Animal.AnimalData> animalList = animal.AnimalList();

			var testAnimal = (from a in animalList
							  //where a.AnimalLocator == 0
							  select a);

			Account account = new Account(@"D:\projects\vetinfo\80702");
			account.AddFilterCriteria(Account.AccountFields.AccountDate, ComparisonType.GreaterThanEqual, new DateTime(2017, 1, 1));
			account.AddFilterCriteria(Account.AccountFields.AccountType, ComparisonType.EqualTo, "I");
			//account.AddFilterCriteria(Account.AccountFields.AccountAmount, ComparisonType.EqualTo, (Decimal)100.00);
			List<Account.AccountData> accountList = account.AccountList();

			//Service service = new Service(@"D:\projects\vetinfo\bstrong");
			//service.AddFilterCriteria(Service.ServiceFields.ServiceDate, ComparisonType.GreaterThanEqual, new DateTime(2008, 1, 1));
			//List<Service.ServiceData> serviceList = service.ServiceList(service);

			//DateTime test = serviceList[0].ServiceDate;

			var clientAccounts = (from accts in accountList
								  join cl in clientList on accts.AccountClient equals cl.ClientId where accts.AccountAnimal > 0
								  //join an in animalList on accts.AccountAnimal equals an.AnimalId
								  from an in animalList
									.Where (a => accts.AccountAnimal == a.AnimalId)
									//.DefaultIfEmpty()
									  //join vc in serviceList on accts.AccountService equals vc.ServiceId
								  select new { cl.ClientId, accts.AccountId, cl.ClientLast, cl.ClientFirst, accts.AccountDate, accts.AccountDescription, accts.AccountInvno, accts.AccountAmount, accts.AccountAnimal, an.AnimalName, an.AnimalBirthday }).ToList().OrderBy(t => t.AccountInvno);

			using (var file = File.CreateText(@"D:\projects\vetinfo\80702\clientAccounts.csv"))
			{
				foreach (var arr in clientAccounts)
				{
					file.WriteLine(string.Join(",", arr.ToString()));
				}
			}

			Console.WriteLine(clientAccounts.Count().ToString() + " Records");
			Console.WriteLine("Running Time: " + ((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString());
			Console.ReadLine();
		}

		//static void Main(string[] args)
		//{
		//	Client client = new Client(@"D:\projects\vetinfo\Hank");
		//	client.AddFilterCriteria(Client.ClientFields.ClientRecd, ComparisonType.EqualTo, "A");
		//	List<Client.ClientData> clientList = client.ClientList(client);

		//	var clientAccounts = (from clients in clientList
		//						  where clients.ClientLast.ToLower() == "smith"
		//						  select clients);
		//}

		//static void Main(string[] args)
		//{
		//	Client client = new Client(@"D:\projects\vetinfo\Hank");
		//	client.ExecuteQuery();
		//	CarsonCRC carsonCRC = new CarsonCRC();

		//	carsonCRC.CreateCRCFile("clientcrc32.dat", client, new List<string>() {Client.ClientFields.ClientFirst, Client.ClientFields.ClientLast });
		//}

		static void Main(string[] args)
		{
			//Account account = new Account(@"D:\projects\vetinfo\bstrong");
			//account.CreateTableCRC("account976.crc", new List<string> { Account.AccountFields.AccountDescription, Account.AccountFields.AccountAmount });
			//var xxxx = account.FindChangedRecords(@"account002.crc", CarsonBackend.DifferentialData.NewAndModifiedRecords, TableInstance.Account);

			//Client client = new Client(@"D:\projects\vetinfo\Hank");
			//client.CreateTableCRC("client.modern.crc", new List<string> { Client.ClientFields.ClientLast, Client.ClientFields.ClientAddress }, TableInstance.Client);

			//Client client = new Client(@"D:\projects\vetinfo\Hank");
			//var xxxx = client.FindChangedRecords(@"client.modern.crc", CarsonBackend.DifferentialData.NewAndModifiedRecords, TableInstance.Client);

			//Animal animal = new Animal(@"D:\projects\vetinfo\bstrong");
			//animal.AddFilterCriteria(Animal.AnimalFields.AnimalLocator, ComparisonType.EqualTo, "duper");
			////animal.AddFilterCriteria(Animal.AnimalFields.AnimalName, ComparisonType.EqualTo, "felix");
			//var stuff = animal.AnimalList();

			//Console.WriteLine(stuff[0].AnimalLocator);

			DateTime now = DateTime.Now;

			Service service = new Service(@"D:\projects\vetinfo\McMurray");
			service.AddFilterCriteria(Service.ServiceFields.ServiceNote, ComparisonType.EqualTo, "vomit");
			var stuff = service.ServiceList();

			double sec = (DateTime.Now - now).TotalMilliseconds / 1000;

			foreach (var x in stuff)
			{
				Console.WriteLine(x.ServiceNote);
			}

			Console.WriteLine("Total Time: " + sec.ToString());

			//Client client = new Client(@"D:\projects\vetinfo\80702");

			//client.AddFilterCriteria(Client.ClientFields.ClientLast, ComparisonType.EqualTo, "smith");
			//List<Client.ClientData> clientData = client.ClientList();
		}

		//static void Main5(string[] args)
		//{
		//	Account account = new Account(@"D:\projects\vetinfo\80702");
		//	List<Account.AccountData> accountData = account.FindChangedRecords("account2.crc", Account.DifferentialData.NewAndModifiedRecords);
		//}

		static void Main6(string[] args)
		{
			Lab lab = new Lab(@"D:\Projects\VetInfo\bstrong");
			List<Lab.LabData> labData = lab.LabList();
			Console.WriteLine(labData[0].LabCompany);
		}

		static void Main7(string[] args)
		{
			Attachment attachment = new Attachment(@"D:\Projects\VetInfo\bstrong");
			List<Attachment.AttachmentData> attachmentData = attachment.AttachmentList();
		}

		static void Main8(string[] args)
		{
			DateTime start = DateTime.Now;
			Account account = new Account(@"D:\Projects\VetInfo\80702");
			//account.AddFilterCriteria(Account.AccountFields.AccountId, ComparisonType.EqualTo, 4078875);
			//account.AddFilterCriteria(Account.AccountFields.AccountInvno, ComparisonType.EqualTo, 90972);
			//account.ExecuteQuery();

			//account.AddFilterCriteria(Account.AccountFields.AccountDate, ComparisonType.GreaterThanEqual, new DateTime(2011, 1, 1));
			//account.AddFilterCriteria(Account.AccountFields.AccountType, ComparisonType.EqualTo, "I");
			//account.AddFilterCriteria(Account.AccountFields.AccountDescription, ComparisonType.EqualTo, "tramadol");
			account.AddFilterCriteria(Account.AccountFields.AccountTranstax, ComparisonType.GreaterThan, 5.0f);
			account.AddFilterCriteria(Account.AccountFields.AccountTranstax, ComparisonType.LessThan, 6.0f);

			List<Account.AccountData> accountData = account.AccountList();
			Console.WriteLine(((DateTime.Now - start).TotalMilliseconds / 1000).ToString());
			Console.WriteLine(accountData[0].AccountTranltax.ToString());
		}

		static void Main9(string[] args)
		{
			Audit audit = new Audit(@"D:\Projects\VetInfo\bstrong");
			audit.AddFilterCriteria(Audit.AuditFields.AuditDate, ComparisonType.GreaterThanEqual, new DateTime(2008, 9, 1));
			//audit.ExecuteQuery();
			List<Audit.AuditData> auditData = audit.AuditList();
		}

		static void Main10(string[] args)
		{
			Follow follow = new Follow(@"D:\Projects\VetInfo\bstrong");
			follow.RetrieveRecords();
			List<Follow.FollowData> followData = follow.FollowList();

			//var fromList = (from f in followData
			//				where f.FollowNote.Length > 0
			//				select f);
		}

		static void Main11(string[] args)
		{
			PurchaseOrder po = new PurchaseOrder(@"D:\Projects\VetInfo\bstrong");
			po.RetrieveRecords();
			List<PurchaseOrder.PurchaseOrderData> poData = po.PurchaseOrderList();
		}

		static void Main12(string[] args)
		{
			Price price = new Price(@"D:\Projects\VetInfo\bstrong");
			price.AddFilterCriteria(Price.PriceFields.PriceSite, ComparisonType.EqualTo, 1);
			//price.ExecuteQuery();
			List<Price.PriceData> priceData = price.PriceList();
		}

		static void Main13(string[] args)
		{
			Appointment appoint = new Appointment(@"d:\projects\vetinfo\bstrong");
			List<Appointment.AppointmentData> appointData = appoint.AppointmentList();

			var request = from a in appointData
						  where a.AppointmentTypeCode.Length > 0
						  select a;

		}

		static void Main15(string[] args)
		{
			//Account account = new Account(@"D:\projects\vetinfo\80702");
			//account.CreateTableCRC("account2.crc", new List<string> { Account.AccountFields.AccountDate, Account.AccountFields.AccountDescription });

			//List<ICrc> accountData = account.FindChangedRecords("account2.crc", Account.DifferentialData.NewAndModifiedRecords, TableInstance.Account);

			Service service = new Service(@"D:\projects\vetinfo\bstrong");
			//service.CreateTableCRC("service.crc", new List<string> { Service.ServiceFields.ServiceDate, Service.ServiceFields.ServiceDescription });

			//List<ICrc> serviceData = service.FindChangedRecords("service.crc", Service.DifferentialData.NewAndModifiedRecords);

		}

		static void Main17(string[] args)
		{
			DateTime start = DateTime.Now;
			Account account = new Account(@"D:\projects\vetinfo\80702");

			account.AddFilterCriteria(Account.AccountFields.AccountDescription, ComparisonType.EqualTo,"prednisolone");
			//account.AddFilterCriteria(Account.AccountFields.AccountDescription, ComparisonType.EqualTo, "prednisone");
			List<Account.AccountData> accountData = account.AccountList();

			Console.WriteLine(((DateTime.Now - start).TotalMilliseconds / 1000).ToString() + " Seconds.");
			Console.WriteLine("Total Records: " + accountData.Count().ToString());
			Console.ReadLine();
		}

		static void MainXXXX(string[] args)
		{
			DateTime start = DateTime.Now;
			Service service = new Service(@"D:\projects\vetinfo\80702");

			service.AddFilterCriteria(Service.ServiceFields.ServiceType, ComparisonType.EqualTo, "D");
			service.AddFilterCriteria(Service.ServiceFields.ServiceDescription, ComparisonType.EqualTo, "kidney");
			List<Service.ServiceData> serviceData = service.ServiceList();

			Console.WriteLine(((DateTime.Now - start).TotalMilliseconds / 1000).ToString() + " Seconds.");
			Console.WriteLine("Total Records: " + serviceData.Count().ToString());
			Console.ReadLine();
		}

		static void Main16(string[] args)
		{
			Client client = new Client(@"D:\projects\vetinfo\80702");

			client.AddFilterCriteria(Client.ClientFields.ClientLast, ComparisonType.EqualTo, "smith");
			//client.AddFilterCriteria(Client.ClientFields.ClientFirst, ComparisonType.EqualTo, "grant");
			List<Client.ClientData> clientData = client.ClientList();
		}

		static void Main18(string[] args)
		{
			Account account = new Account(@"D:\projects\vetinfo\80702");

			List<Account.AccountData> accountData = account.AccountList();

			//string description = accountData[0].AccountDescription;
			Account.AccountData accountRecord = accountData[0];
			Console.WriteLine(accountRecord.AccountDate.ToString());
		}

		static void Main19(string[] args)
		{
			Audit audit = new Audit(@"D:\projects\vetinfo\bstrong");
			List<Audit.AuditData> auditData = audit.AuditList();

			foreach (Audit.AuditData auditRecord in auditData)
			{
				Console.WriteLine(auditRecord.AuditComments);
			}


			//Console.WriteLine(auditData[0].AuditId.ToString());
			Console.ReadLine();
		}

		static void Main20(string[] args)
		{
			Client client = new Client(@"D:\projects\vetinfo\bstrong");
			List<Client.ClientData> clientData = client.ClientList();

			Console.WriteLine(clientData[2].ClientEmail.ToString());
			Console.ReadLine();
		}

		static void Main21(string[] args)
		{
			Follow follow = new Follow(@"D:\projects\vetinfo\bstrong");
			List<Follow.FollowData> followData = follow.FollowList();

			Console.WriteLine(followData[0].FollowSubject);
			Console.ReadLine();
		}

		static void Main22(string[] args)
		{
			Animal animal = new Animal(@"D:\projects\vetinfo\bstrong");
			List<Animal.AnimalData> animalData = animal.AnimalList();

			Console.WriteLine(animalData[2].AnimalLocator);
			Console.ReadLine();
		}		

		static void Main23(string[] args)
		{
			Service service = new Service(@"D:\projects\vetinfo\bstrong");
			List<Service.ServiceData> serviceData = service.ServiceList();
			Console.WriteLine(serviceData[0].ServiceNote);
			Console.ReadLine();
		}

	}
}
