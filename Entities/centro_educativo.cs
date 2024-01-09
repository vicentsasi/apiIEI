using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practiquesIEI.Entities
{
    public class centro_educativo
    {
        public string nombre { get; set; }
        public tipo_centro tipo { get; set; }
        public string longitud { get; set; }
        public string latitud { get; set; }
        public int telefono { get; set; }
        public string descripcion { get; set; }
        public string direccion { get; set; }
        public string cod_postal { get; set; }
        public string loc_codigo { get; set; }
        public string loc_nombre { get; set; }
        public string prov_nombre { get; set; }

        public centro_educativo() { }
        public centro_educativo(string nombre, tipo_centro tipo, string longitud, string latitud, int telefono, string descripcion, string direccion, string cod_postal, string loc_codigo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.longitud = longitud;
            this.latitud = latitud;
            this.telefono = telefono;
            this.descripcion = descripcion;
            this.direccion = direccion;
            this.cod_postal = cod_postal;
            this.loc_codigo = loc_codigo;
        }
    }
}
