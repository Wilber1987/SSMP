using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;
using CAPA_DATOS.Security;
using DataBaseModel;

namespace BusinessLogic.IA
{
    public class BlackListServices
    {
        public static string BlackListDefault = "";
        public static bool IsPermitUser(string? userId)
        {
            string blackList = Transactional_Configuraciones
                .GetParam(ConfiguracionesThemeEnum.TEMPLATE_NAME, BlackListDefault)?
                .Valor ?? BlackListDefault;

            // Convertimos la lista negra en un conjunto de usuarios, asegurándonos de eliminar espacios en blanco.
            HashSet<string> blackListUsers = [.. blackList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

            return userId != null && !blackListUsers.Contains(userId.Trim());
        }
        public static ResponseService AddUser(int? Id_Perfil)
        {
            if (Id_Perfil != null)
            {
                Transactional_Configuraciones blackList = Transactional_Configuraciones
                     .GetParam(ConfiguracionesThemeEnum.BLACK_LIST, BlackListDefault);
                Tbl_Profile? profile = new Tbl_Profile { Id_Perfil = Id_Perfil }.Find<Tbl_Profile>();
                if (profile != null
                && profile?.Correo_institucional != null
                && blackList.Valor != null
                && !blackList.Valor.Contains(profile.Correo_institucional))
                {
                    blackList.Valor += ", " + profile.Correo_institucional;
                    return blackList.Update();
                }
            }
            return new ResponseService
            {
                status = 400,
                message = "remitente no existe"
            };
        }
        public static ResponseService RemoveUser(int? Id_Perfil)
        {
            if (Id_Perfil != null)
            {
                Transactional_Configuraciones blackList = Transactional_Configuraciones
                     .GetParam(ConfiguracionesThemeEnum.BLACK_LIST, BlackListDefault);
                Tbl_Profile? profile = new Tbl_Profile { Id_Perfil = Id_Perfil }.Find<Tbl_Profile>();
                if (profile != null
                && profile?.Correo_institucional != null
                && blackList.Valor != null
                && blackList.Valor.Contains(profile.Correo_institucional))
                {
                    blackList.Valor = blackList.Valor
                        .Replace(", " + profile.Correo_institucional, "")
                        .Replace(profile.Correo_institucional, "");
                    return blackList.Update();
                }
            }
            return new ResponseService
            {
                status = 400,
                message = "remitente no existe"
            };
        }

        public static object IsInBlackList(int? Id_Perfil)
        {
            if (Id_Perfil != null)
            {
                Transactional_Configuraciones blackList = Transactional_Configuraciones
                     .GetParam(ConfiguracionesThemeEnum.BLACK_LIST, BlackListDefault);
                Tbl_Profile? profile = new Tbl_Profile { Id_Perfil = Id_Perfil }.Find<Tbl_Profile>();
                if (profile != null
                && profile?.Correo_institucional != null
                && blackList.Valor != null
                && blackList.Valor.Contains(profile.Correo_institucional))
                {
                    return new ResponseService
                    {
                        status = 200,
                        body = true
                    };
                }
            }
            return new ResponseService
            {
                status = 200,
                body = false
            };
        }
    }
}