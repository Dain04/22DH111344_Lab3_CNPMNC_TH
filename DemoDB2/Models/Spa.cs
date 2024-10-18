

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class Spa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Spa()
        {
            this.DichVuSuDung = new HashSet<DichVuSuDung>();
        }
    
        public int SpaID { get; set; }
        public string TenDichVu { get; set; }
        public string MoTa { get; set; }
        public Nullable<decimal> GiaDichVu { get; set; }
        public Nullable<int> ThoiGianDichVu { get; set; }
        public string ImageSpa { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVuSuDung> DichVuSuDung { get; set; }
    }
}
