using SQLite;
using YAP.Libs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIMiniApp.Models;

namespace MAUIMiniApp.Data
{
    public static class AccountDatabase
    {
        static SQLiteAsyncConnection Database;

        async static Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<Account>();
        }

        public static async Task<int> SaveItemAsync(Account item)
        {
            await Init();

            var res = await Database.Table<Account>().Where(x => x.CompanyCode == item.CompanyCode && x.AccountNo == item.AccountNo).FirstOrDefaultAsync();
            if (res != null)
            {
                return await Database.UpdateAsync(item);
            }
            else
            {
                return await Database.InsertAsync(item);
            }
        }

        public static async Task<int> DeleteItemAsync(Account item)
        {
            await Init();
            return await Database.DeleteAsync(item);
        }

        public static async Task<List<Account>> GetItemsAsync()
        {
            await Init();
            return await Database.Table<Account>().ToListAsync();
        }

        public static async Task<int> TruncateItemAsync()
        {
            try
            {
                var resList = await GetItemsAsync();
                foreach (var item in resList)
                {
                    await DeleteItemAsync(item);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
