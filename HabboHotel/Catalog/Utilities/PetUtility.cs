using System;
using ProjectHub.Database.Interfaces;
using ProjectHub.HabboHotel.Rooms.AI;


namespace ProjectHub.HabboHotel.Items.Utilities
{
    public static class PetUtility
    {
        public static bool IsPet(InteractionType Type)
        {
            if (Type == InteractionType.pet0 || Type == InteractionType.pet1 || Type == InteractionType.pet2 || Type == InteractionType.pet3 || Type == InteractionType.pet4 || Type == InteractionType.pet5 || Type == InteractionType.pet6 || Type == InteractionType.pet7 || Type == InteractionType.pet8 || Type == InteractionType.pet9 || Type == InteractionType.pet10 || Type == InteractionType.pet11 || Type == InteractionType.pet12 || Type == InteractionType.pet13 || Type == InteractionType.pet14 || Type == InteractionType.pet15 || Type == InteractionType.pet16 || Type == InteractionType.pet17 || Type == InteractionType.pet18)
            {
                return true;
            }

            return false;
        }

        public static bool CheckPetName(string PetName)
        {
            if (PetName.Length < 1 || PetName.Length > 16)
            {
                return false;
            }

            if (!ProjectHub.IsValidAlphaNumeric(PetName))
            {
                return false;
            }

            return true;
        }

        public static Pet CreatePet(int UserId, string Name, int Type, string Race, string Color)
        {
            Pet Pet = new Pet(0, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, ProjectHub.GetUnixTimestamp(), 0, 0, 0.0, 0, 0, 0, -1, "-1");

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("INSERT INTO " + ProjectHub.DbPrefix + "bots (user_id,name, ai_type) VALUES (" + Pet.OwnerId + ",@" + Pet.PetId + "name, 'pet')");
                DbClient.AddParameter(Pet.PetId + "name", Pet.Name);
                Pet.PetId = Convert.ToInt32(DbClient.InsertQuery());

                DbClient.SetQuery("INSERT INTO " + ProjectHub.DbPrefix + "bots_petdata (id,type,race,color,experience,energy,createstamp) VALUES (" + Pet.PetId + ", " + Pet.Type + ",@" + Pet.PetId + "race,@" + Pet.PetId + "color,0,100,UNIX_TIMESTAMP())");
                DbClient.AddParameter(Pet.PetId + "race", Pet.Race);
                DbClient.AddParameter(Pet.PetId + "color", Pet.Color);
                DbClient.RunQuery();
            }

            return Pet;
        }
    }
}
