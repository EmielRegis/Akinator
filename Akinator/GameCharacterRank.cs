//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Akinator
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameCharacterRank
    {
        public int Id { get; set; }
        public int Character { get; set; }
        public Nullable<double> ProbabilisticEvaluation { get; set; }
        public Nullable<int> RankPoints { get; set; }
    
        public virtual Character Character1 { get; set; }
    }
}
