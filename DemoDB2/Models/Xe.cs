
namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class Xe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Xe()
        {
            this.DichVuSuDung = new HashSet<DichVuSuDung>();
        }
    
        public int XeID { get; set; }
        public string HieuXe { get; set; }
        public string BienSoXe { get; set; }
        public string TaiXe { get; set; }
        public Nullable<int> SoChoNgoi { get; set; }
        public Nullable<decimal> GiaXe { get; set; }
        public string ImageXe { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVuSuDung> DichVuSuDung { get; set; }
    }
}
