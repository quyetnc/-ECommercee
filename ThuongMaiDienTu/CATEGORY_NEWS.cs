//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThuongMaiDienTu
{
    using System;
    using System.Collections.Generic;
    
    public partial class CATEGORY_NEWS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORY_NEWS()
        {
            this.NEWS = new HashSet<NEWS>();
        }
    
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIcon { get; set; }
        public Nullable<int> IdUser { get; set; }
    
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NEWS> NEWS { get; set; }
    }
}
