using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;
using CAPA_DATOS.BDCore.Abstracts;

namespace BusinessLogic.Rastreo.Model
{
    public class BDConnection
    {
        public WDataMapper? wDataMapper = SqlADOConexion.BuildDataMapper("localhost", "sa", "zaxscd", "");
    }
}