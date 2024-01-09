using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practiquesIEI.Entities
{
    public class provincia
    {
        public string codigo { get; set; }
        public string nombre { get; set; }

        public provincia() { }
        public provincia(string codigo, string nombre) { 
            this.codigo = codigo;
            this.nombre = nombre;
        }
    }
}
