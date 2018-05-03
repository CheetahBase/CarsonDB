using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsonDB
{
	public class Animal : CarsonBackend
	{
		private enum AnimalFieldOrdinals
		{
			Id,
			AnimalName,
			AnimalRecd,
			AnimalCodes,
			AnimalAdded,
			AnimalRabies,
			AnimalBirthday,
			AnimalVisit,
			AnimalFlags,
			AnimalSex,
			AnimalAllergy,
			AnimalBreed,
			AnimalSpecies,
			AnimalRegistration,
			AnimalWeight,
			AnimalMeasure,
			AnimalColor,
			AnimalClient,
			AnimalNote,
			AnimalPhoto,
			AnimalSuspend,
			AnimalDeath,
			AnimalLocator,
			AnimalLastCompany
		}

		public static class AnimalFields
		{
			public const string Id = "ID";
			public const string AnimalName = "ANIMAL_NAME";
			public const string AnimalRecd = "ANIMAL_RECD";
			public const string AnimalCodes = "ANIMAL_CODES";
			public const string AnimalAdded = "ANIMAL_ADDED";
			public const string AnimalRabies = "ANIMAL_RABIES";
			public const string AnimalBirthday = "ANIMAL_BIRTHDAY";
			public const string AnimalVisit = "ANIMAL_VISIT";
			public const string AnimalFlags = "ANIMAL_FLAGS";
			public const string AnimalSex = "ANIMAL_SEX";
			public const string AnimalAllergy = "ANIMAL_ALLERGY";
			public const string AnimalBreed = "ANIMAL_BREED";
			public const string AnimalSpecies = "ANIMAL_SPECIES";
			public const string AnimalRegistration = "ANIMAL_REGISTRATION";
			public const string AnimalWeight = "ANIMAL_WEIGHT";
			public const string AnimalMeasure = "ANIMAL_MEASURE";
			public const string AnimalColor = "ANIMAL_COLOR";
			public const string AnimalClient = "ANIMAL_CLIENT";
			public const string AnimalNote = "ANIMAL_NOTE";
			public const string AnimalPhoto = "ANIMAL_PHOTO";
			public const string AnimalSuspend = "ANIMAL_SUSPEND";
			public const string AnimalDeath = "ANIMAL_DEATH";
			public const string AnimalLocator = "ANIMAL_LOCATOR";
			public const string AnimalLastCompany = "ANIMAL_LAST_COMPANY";
		}

		public class AnimalData : ICrc
		{
			private int _recordNumber;
			private Animal _animal;

			public AnimalData(Animal animal, int recordNumber)
			{
				_animal = animal;
				_recordNumber = recordNumber;
			}

			public int Id
			{
				get
				{
					int animalId = 0;

					try
					{
						animalId = _animal.IntegerFieldValue(AnimalFieldOrdinals.Id, _recordNumber);
						return animalId;
					}
					catch (RecordLockedException)
					{
						animalId = _recordNumber;
						this.RecordState = RecordStatus.Error;
						return animalId;
					}
				}
			}

			public string AnimalName
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalName, _recordNumber);
				}
			}

			public string AnimalRecd
			{
				get
				{
					return _animal.CharacterFieldValue(AnimalFieldOrdinals.AnimalRecd, _recordNumber);
				}
			}

			public string AnimalCodes
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalCodes, _recordNumber);
				}
			}

			public DateTime AnimalAdded
			{
				get
				{
					return _animal.DateFieldValue(AnimalFieldOrdinals.AnimalAdded, _recordNumber);
				}
			}

			public string AnimalRabies
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalRabies, _recordNumber);
				}
			}

			public DateTime AnimalBirthday
			{
				get
				{
					return _animal.DateFieldValue(AnimalFieldOrdinals.AnimalBirthday, _recordNumber);
				}
			}

			public int AnimalVisit
			{
				get
				{
					return _animal.IntegerFieldValue(AnimalFieldOrdinals.AnimalVisit, _recordNumber);
				}
			}

			public int AnimalFlags
			{
				get
				{
					return _animal.IntegerFieldValue(AnimalFieldOrdinals.AnimalFlags, _recordNumber);
				}
			}

			public string AnimalSex
			{
				get
				{
					return _animal.CharacterFieldValue(AnimalFieldOrdinals.AnimalSex, _recordNumber);
				}
			}

			public string AnimalAllergy
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalAllergy, _recordNumber);
				}
			}

			public string AnimalBreed
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalBreed, _recordNumber);
				}
			}

			public string AnimalSpecies
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalSpecies, _recordNumber);
				}
			}

			public string AnimalRegistration
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalRegistration, _recordNumber);
				}
			}

			public Decimal AnimalWeight
			{
				get
				{
					return _animal.FloatFieldValue(AnimalFieldOrdinals.AnimalWeight, _recordNumber);
				}
			}

			public string AnimalMeasure
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalMeasure, _recordNumber);
				}
			}

			public string AnimalColor
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalColor, _recordNumber);
				}
			}

			public int AnimalClient
			{
				get
				{
					return _animal.IntegerFieldValue(AnimalFieldOrdinals.AnimalClient, _recordNumber);
				}
			}

			public string AnimalNote
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalNote, _recordNumber);
				}
			}

			public string AnimalPhoto
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalPhoto, _recordNumber);
				}
			}

			public DateTime AnimalSuspend
			{
				get
				{
					return _animal.DateFieldValue(AnimalFieldOrdinals.AnimalSuspend, _recordNumber);
				}
			}

			public DateTime AnimalDeath
			{
				get
				{
					return _animal.DateFieldValue(AnimalFieldOrdinals.AnimalDeath, _recordNumber);
				}
			}

			public string AnimalLocator
			{
				get
				{
					return _animal.StringFieldValue(AnimalFieldOrdinals.AnimalLocator, _recordNumber);
				}
			}

			public int AnimalLastCompany
			{
				get
				{
					return _animal.IntegerFieldValue(AnimalFieldOrdinals.AnimalLastCompany, _recordNumber);
				}
			}

			[System.ComponentModel.DefaultValue(RecordStatus.None)]
			public RecordStatus RecordState
			{
				get;
				set;
			}
		}

		public Animal()
		{
			this.AddDatabaseDefinitions(CarsonStaticSettings.DatabasePath);
		}

		public Animal(string databasePath)
		{
			AddDatabaseDefinitions(databasePath);
			CarsonStaticSettings.DatabasePath = databasePath;
		}

		private void AddDatabaseDefinitions(string databasePath)
		{
			this.DatabaseName = "ANIMAL";
			this.DatabasePath = databasePath;
			this.TableId = TableInstance.Animal;

			this.AddFieldDefinition(AnimalFields.Id, AnimalFieldOrdinals.Id, AVImarkDataType.AVImarkAutoNumber);
			this.AddFieldDefinition(AnimalFields.AnimalName, AnimalFieldOrdinals.AnimalName, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalRecd, AnimalFieldOrdinals.AnimalRecd, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AnimalFields.AnimalCodes, AnimalFieldOrdinals.AnimalCodes, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalAdded, AnimalFieldOrdinals.AnimalAdded, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AnimalFields.AnimalRabies, AnimalFieldOrdinals.AnimalRabies, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalBirthday, AnimalFieldOrdinals.AnimalBirthday, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AnimalFields.AnimalVisit, AnimalFieldOrdinals.AnimalVisit, AVImarkDataType.AVImarkWord);
			this.AddFieldDefinition(AnimalFields.AnimalFlags, AnimalFieldOrdinals.AnimalFlags, AVImarkDataType.AVImarkByte);
			this.AddFieldDefinition(AnimalFields.AnimalSex, AnimalFieldOrdinals.AnimalSex, AVImarkDataType.AVImarkCharacter);
			this.AddFieldDefinition(AnimalFields.AnimalAllergy, AnimalFieldOrdinals.AnimalAllergy, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalBreed, AnimalFieldOrdinals.AnimalBreed, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalSpecies, AnimalFieldOrdinals.AnimalSpecies, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalRegistration, AnimalFieldOrdinals.AnimalRegistration, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalWeight, AnimalFieldOrdinals.AnimalWeight, AVImarkDataType.AVImarkSingle);
			this.AddFieldDefinition(AnimalFields.AnimalMeasure, AnimalFieldOrdinals.AnimalMeasure, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalColor, AnimalFieldOrdinals.AnimalColor, AVImarkDataType.AVImarkDynamicString);
			this.AddFieldDefinition(AnimalFields.AnimalClient, AnimalFieldOrdinals.AnimalClient, AVImarkDataType.AVImarkDoubleWord);
			this.AddFieldDefinition(AnimalFields.AnimalNote, AnimalFieldOrdinals.AnimalNote, AVImarkDataType.AVImarkLinkToWp);
			this.AddFieldDefinition(AnimalFields.AnimalPhoto, AnimalFieldOrdinals.AnimalPhoto, AVImarkDataType.AVImarkLinkToPhrase);
			this.AddFieldDefinition(AnimalFields.AnimalSuspend, AnimalFieldOrdinals.AnimalSuspend, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AnimalFields.AnimalDeath, AnimalFieldOrdinals.AnimalDeath, AVImarkDataType.AVImarkDate);
			this.AddFieldDefinition(AnimalFields.AnimalLocator, AnimalFieldOrdinals.AnimalLocator, AVImarkDataType.AVImarkLinkToPhrase);
			this.AddFieldDefinition(AnimalFields.AnimalLastCompany, AnimalFieldOrdinals.AnimalLastCompany, AVImarkDataType.AVImarkWord);
		}

		public List<AnimalData> AnimalList()
		{
			List<AnimalData> animalList = new List<AnimalData>();
			this.Open();

			int recordCount = this.RetrieveRecords();
			this.MoveFirst();

			for (int x = 1; x <= recordCount; x++)
			{
				AnimalData animalData = new AnimalData(this, x);
				animalList.Add(animalData);
			}

			return animalList;
		}
	}
}
