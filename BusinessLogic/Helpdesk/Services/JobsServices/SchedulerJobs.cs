

using BusinessLogic.Notificaciones_Mensajeria.Gestion_Notificaciones.Operations;
using APPCORE.Cron.Jobs;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
using CAPA_NEGOCIO.Services;
using DataBaseModel;
using Microsoft.Extensions.Logging;

namespace BackgroundJob.Cron.Jobs
{
	public class MySchedulerJob : CronBackgroundJob
	{
		private readonly ILogger<MySchedulerJob> _log;

		public MySchedulerJob(CronSettings<MySchedulerJob> settings, ILogger<MySchedulerJob> log)
			: base(settings.CronExpression, settings.TimeZone)
		{
			_log = log;
		}

		protected override Task DoWork(CancellationToken stoppingToken)
		{
			_log.LogInformation(":::::::::::Running... at {0}", DateTime.UtcNow);
			//CARGA AUTOMATICA DE CASOS
			try
			{
				new IMAPCaseServices().chargeAutomaticCase();
			}
			catch (Exception ex)
			{
				_log.LogInformation(":::::::::::ERROR... at {0}", ex);
			}

			return Task.CompletedTask;
		}
	}

	public class CreateAutomaticsCaseSchedulerJob : CronBackgroundJob
	{
		private readonly ILogger<CreateAutomaticsCaseSchedulerJob> _log;

		public CreateAutomaticsCaseSchedulerJob(CronSettings<CreateAutomaticsCaseSchedulerJob> settings, ILogger<CreateAutomaticsCaseSchedulerJob> log)
			: base(settings.CronExpression, settings.TimeZone)
		{
			_log = log;
		}

		protected override async Task<Task> DoWork(CancellationToken stoppingToken)
		{
			_log.LogInformation(":::::::::::Running... CreateAutomaticsCaseSchedulerJob at {0}", DateTime.UtcNow);
			//CARGA AUTOMATICA DE CASOS
			try
			{
				if(SystemConfig.IsAutomaticCaseActive()) 
				{
					await new IMAPCaseServices().chargeAutomaticCase();
				}				
				await new SmtpHelpdeskServices().sendCaseMailNotificationsAsync();
			}
			catch (System.Exception ex)
			{
				_log.LogInformation(":::::::::::ERROR... at {0}", ex);
			}
			return Task.CompletedTask;
		}
	}

	public class SendMailNotificationsSchedulerJob : CronBackgroundJob
	{
		private readonly ILogger<SendMailNotificationsSchedulerJob> _log;

		public SendMailNotificationsSchedulerJob(CronSettings<SendMailNotificationsSchedulerJob> settings, ILogger<SendMailNotificationsSchedulerJob> log)
			: base(settings.CronExpression, settings.TimeZone)
		{
			_log = log;
		}

		protected override async Task<Task> DoWork(CancellationToken stoppingToken)
		{
			_log.LogInformation(":::::::::::Running...  SendMailNotificationsSchedulerJob at {0}", DateTime.UtcNow);
			//CARGA AUTOMATICA DE CASOS
			try
			{
				await new SmtpHelpdeskServices().sendCaseMailNotificationsAsync();
				//ENVIO DE NOTIFICACIONES
				await NotificationSenderOperation.SendNotificationsAsync();
			}
			catch (Exception ex)
			{
				_log.LogInformation(":::::::::::ERROR... at {0}", ex);
			}

			return Task.CompletedTask;
		}
	}
	
	public class ReportSenderSchedulerJob : CronBackgroundJob
	{
		private readonly ILogger<ReportSenderSchedulerJob> _log;

		public ReportSenderSchedulerJob(CronSettings<ReportSenderSchedulerJob> settings, ILogger<ReportSenderSchedulerJob> log)
			: base(settings.CronExpression, settings.TimeZone)
		{
			_log = log;
		}

		protected override async Task<Task> DoWork(CancellationToken stoppingToken)
		{
			_log.LogInformation(":::::::::::Running...  ReportSenderSchedulerJob at {0}", DateTime.UtcNow);
			//CARGA AUTOMATICA DE CASOS
			try
			{				
				NotificationSenderOperation.SendNotificationReport();
				await Task.Delay(60000);
			}
			catch (Exception ex)
			{
				_log.LogInformation(":::::::::::ERROR... at {0}", ex);
			}

			return Task.CompletedTask;
		}
	}
}

