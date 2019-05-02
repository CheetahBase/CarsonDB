using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public enum AVImarkDataType
	{
		AVImarkAutoNumber,
		AVImarkCharacter,
		AVImarkDynamicString,
		AVImarkDate,
		AVImarkTime,
		AVImarkSingle,
		AVImarkDouble,
		AVImarkWord,
		AVImarkDoubleWord,
		AVImarkByte,
		AVImarkBit,
		AVImarkBool,
		AVImarkInteger,
		AVImarkImpliedDecimal,
		AVImarkImpliedDecimal2,
		AVImarkSignedImpliedDecimal,
		AVImarkLongInteger,
		AVImarkLinkToPhrase,
		AVImarkLinkToWp
	}

	public class Definition
	{
		public string FieldName;
		public Enum FieldOrdinal;
		public AVImarkDataType FieldType;
		public Decimal Multiplier = 0;
	}

	public enum ComparisonType
	{
		EqualTo,
		GreaterThan,
		GreaterThanEqual,
		LessThan,
		LessThanEqual,
		NotEqualTo,
		Contains
	}

	public enum RecordStatus
	{
		Added,
		Modified,
		None,
		Error
	}

	public enum TableInstance
	{
		Account,
		Animal,
		Appointment,
		Attach,
		Audit,
		Client,
		Diagnose,
		Entry,
		Follow,
		Item,
		Lab,
		PurchaseOrder,
		Price,
		Problem,
		Quotail,
		Quote,
		Reminder,
		Service,
		Split,
		Table,
		Test,
		Treatment,
		User,
		Medical,
		MiscDirect = 1000
	}

	public interface ICrc
	{
		[System.ComponentModel.DefaultValue(RecordStatus.None)]
		RecordStatus RecordState
		{
			get;
			set;
		}
	}

	public static class ExtensionMethods
	{
		public static ICrc GetTableInstance(TableInstance tableInstance, CarsonBackend instance, int recordNumber)
		{
			switch (tableInstance)
			{
				case TableInstance.Account:
					return new Account.AccountData((Account)instance, recordNumber);

				case TableInstance.Animal:
					return new Animal.AnimalData((Animal)instance, recordNumber);

				case TableInstance.Appointment:
					return new Appointment.AppointmentData((Appointment)instance, recordNumber);

				case TableInstance.Attach:
					return new Attachment.AttachmentData((Attachment)instance, recordNumber);

				case TableInstance.Audit:
					return new Audit.AuditData((Audit)instance, recordNumber);

				case TableInstance.Client:
					return new Client.ClientData((Client)instance, recordNumber);

				case TableInstance.Diagnose:
					return new Diagnose.DiagnoseData((Diagnose)instance, recordNumber);

				case TableInstance.Entry:
					return new Entry.EntryData((Entry)instance, recordNumber);

				case TableInstance.Follow:
					return new Follow.FollowData((Follow)instance, recordNumber);

				case TableInstance.Item:
					return new Item.ItemData((Item)instance, recordNumber);

				case TableInstance.Lab:
					return new Lab.LabData((Lab)instance, recordNumber);

				case TableInstance.Problem:
					return new Problem.ProblemData((Problem)instance, recordNumber);

				case TableInstance.PurchaseOrder:
					return new PurchaseOrder.PurchaseOrderData((PurchaseOrder)instance, recordNumber);

				case TableInstance.Price:
					return new Price.PriceData((Price)instance, recordNumber);

				case TableInstance.Quotail:
					return new Quotail.QuotailData((Quotail)instance, recordNumber);

				case TableInstance.Quote:
					return new Quote.QuoteData((Quote)instance, recordNumber);

				case TableInstance.Reminder:
					return new Reminder.ReminderData((Reminder)instance, recordNumber);

				case TableInstance.Service:
					return new Service.ServiceData((Service)instance, recordNumber);

				case TableInstance.Split:
					return new Split.SplitData((Split)instance, recordNumber);

				case TableInstance.Table:
					return new Table.TableData((Table)instance, recordNumber);

				case TableInstance.Test:
					return new Test.TestData((Test)instance, recordNumber);

				case TableInstance.Treatment:
					return new Treatment.TreatmentData((Treatment)instance, recordNumber);

				case TableInstance.User:
					return new User.UserData((User)instance, recordNumber);

				default:
					return null;
			}
		}
	}
}