﻿using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace QuickStart.UWP.Data
{
    public class DataTable<T>: ObservableCollection<T>
    {
        private static IMobileServiceSyncTable<T> _controller = null;

        /// <summary>
        /// Initialize the _controller table.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            if (_controller == null)
            {
                var store = await DataStore.GetInstance();
                _controller = store.CloudService.GetSyncTable<T>();
            }
        }

        /// <summary>
        /// Add a record to the async table
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public async Task CreateAsync(T row)
        {
            // Add the item to the observable collection
            Add(row);

            // Add the item to the sync table
            await InitializeAsync();
            await _controller.InsertAsync(row);
        }

        /// <summary>
        /// Update a record in the async table
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public async Task UpdateAsync(T row)
        {
            // Update the record in the observable collection
            for (var idx = 0; idx < Count; idx++)
            {
                if (Items[idx].Equals(row))
                {
                    Items[idx] = row;
                }
            }

            // Update the record in the sync table
            await InitializeAsync();
            await _controller.UpdateAsync(row);

        }

        /// <summary>
        /// Delete a record in the sync table
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public async Task DeleteAsync(T row)
        {
            // Remove this item from the observable collection
            Remove(row);

            // Remove this item from the sync table
            await InitializeAsync();
            await _controller.DeleteAsync(row);

        }

        /// <summary>
        /// Refresh the async table from the cloud
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            await InitializeAsync();

            // Do the Pushes
            var store = await DataStore.GetInstance();
            await store.CloudService.SyncContext.PushAsync();

            // Do the pulls
            await _controller.PullAsync("tablequery", _controller.CreateQuery());
        }
    }
}