using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container2
{
    public class Container2Service : IContainer2
    {
        string InstanceId;
        IReliableDictionary<string, int> ContainerCounter;
        IReliableStateManager StateManager;
        public Container2Service()
        {

        }

        public Container2Service(IReliableStateManager stateManager, string id)
        {
            InstanceId = id;
            StateManager = stateManager;
        }
        public async Task<bool> ContainerCall()
        {
            ContainerCounter = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Container2Counter");

            using (var tx = this.StateManager.CreateTransaction())
            {
                int i = (await ContainerCounter.TryGetValueAsync(tx, "container2:" + InstanceId)).Value;
                await ContainerCounter.TryRemoveAsync(tx, "container2:" + InstanceId);
                i++;
                await ContainerCounter.TryAddAsync(tx, "container2:" + InstanceId, i);
                await tx.CommitAsync();
            }
            bool a = true;
            return a;
        }
    }
}
