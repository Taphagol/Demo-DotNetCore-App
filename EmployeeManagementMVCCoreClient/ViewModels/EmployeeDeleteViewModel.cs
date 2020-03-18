using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementMVCCoreClient.ViewModels
{
    public class EmployeeDeleteViewModel : EmployeeCreateViewModel
    {
        public int Id { get; set; }

        public string ExistingPhotoPath { get; set; }
    }
}
