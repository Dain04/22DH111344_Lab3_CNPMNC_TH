
namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class NhanVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhanVien()
        {
            this.DichVuSuDung = new HashSet<DichVuSuDung>();
            this.LichLamViec = new HashSet<LichLamViec>();
            this.Luong1 = new HashSet<Luong>();
        }
    
        public int NhanVienID { get; set; }
        public Nullable<int> ChucVuID { get; set; }
        public Nullable<decimal> Luong { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string ImageNhanVien { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
        public virtual ChucVu ChucVu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVuSuDung> DichVuSuDung { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichLamViec> LichLamViec { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Luong> Luong1 { get; set; }
        public string TenChucVu { get; internal set; }
    }
}
