using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Compute
{
    [ServiceContract]
    public interface ICompute
    {
        [OperationContract]
        Task<bool> ComputeAsync(int container1, int container2, int container3);
    }
}
