using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Container1
{
    [ServiceContract]
    public interface IContainer1
    {
        [OperationContract]
        Task<bool> ContainerCall();
    }
}
