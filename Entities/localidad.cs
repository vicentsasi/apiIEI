using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practiquesIEI.Entities
{
    public class localidad
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string prov_codigo { get; set; }

        public localidad() { }
        public localidad(string codigo, string nombre, string prov_codigo)
        {
            this.codigo = codigo;
            this.nombre = nombre;
            this.prov_codigo = prov_codigo;
        }
    }
}
