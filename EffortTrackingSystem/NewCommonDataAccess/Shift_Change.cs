//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewCommonDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shift_Change
    {
        public int shift_Change_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> assigned_shift_id { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<int> new_shift_id { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
    
        public virtual Shift Shift { get; set; }
        public virtual Shift Shift1 { get; set; }
        public virtual User User { get; set; }
    }
}