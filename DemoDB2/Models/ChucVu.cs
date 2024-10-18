

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChucVu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChucVu()
        {
            this.NhanVien = new HashSet<NhanVien>();
        }
        public List<ChucVu> ListChucVu { get; internal set; }
        public int ChucVuID { get; set; }
        public string TenChucVu { get; set; }
        public string MoTaChucVu { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhanVien> NhanVien { get; set; }
    }
}
