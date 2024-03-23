using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies_Core_Layer.Models
{
    public class AttachmentsOptions
    {
        public string AllowedExtensions { get; set; }
        public int MaxSizeInMBs { get; set; }
        public bool EnableCompression { get; set; }


    }
}
