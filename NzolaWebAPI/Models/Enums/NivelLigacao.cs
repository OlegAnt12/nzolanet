using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Models
{
    public class Seguidor
    {
        //- Id: int
        //- SeguidorId: int
       //- SeguidoId: int
      //- DataInicio: DateTime
     public int Id { get; set; }

     public  int SeguidorId { get; set; }

     public int SeguidoId { get; set; }

     public DateTime DataInicio { get; set; } = DateTime.Now;

    }
}