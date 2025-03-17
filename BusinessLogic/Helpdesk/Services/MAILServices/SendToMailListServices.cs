using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_NEGOCIO.MAPEO;
using CAPA_NEGOCIO.Util;
using DataBaseModel;
using MimeKit.Utils;

namespace BusinessLogic.Helpdesk.Services.MAILServices
{
    public class SendToMailListServices
    {
        public static void CreateMailForAsignedCase(List<Tbl_Profile?> users, Tbl_Case Tbl_Case)
		{			
			new Tbl_Mails()
			{
				Id_Case = Tbl_Case?.Id_Case,
				Subject = $"Alerta: Nuevo caso asignado requiere atención: " + Tbl_Case?.Titulo?.ToUpper(),
				Body = $"Nuevo caso asignado requiere atención: {Tbl_Case?.Titulo?.ToUpper()} a las {DateUtil.GetDateTimeName(Tbl_Case?.Fecha_Inicio)}",
				FromAdress = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "User"),
				Estado = MailState.PENDIENTE.ToString(),
				Date = DateTime.Now,
				Uid = Tbl_Case?.MimeMessageCaseData?.MessageId,
				ToAdress = users.Select(user =>  user?.Correo_institucional ?? "nodefined@correo.net").ToList(),
			}.Save();
		}
    }
}