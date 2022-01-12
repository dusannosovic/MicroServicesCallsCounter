using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Container2
{
    [ServiceContract]
    public interface IContainer2
    {
        [OperationContract]
        Task<bool> ContainerCall();
    }
}
