﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAPA_NEGOCIO
{
	public class MessageContent { public string Role { get; set; } public string Content { get; set; } }

	public class JsonResponse
	{
		public string Model { get; set; }
		public DateTime CreatedAt { get; set; }
		public MessageContent Message { get; set; }
		public string DoneReason { get; set; }
		public bool Done { get; set; }
		public long TotalDuration { get; set; }
		public long LoadDuration { get; set; }
		public int PromptEvalCount { get; set; }
		public long PromptEvalDuration { get; set; }
		public int EvalCount { get; set; }
		public long EvalDuration { get; set; }
		public bool? WithAgentResponse { get; set; }
	}

	public class ResponseWebApi
	{
		public string? Reply { get; set; }
		public bool? WithAgentResponse { get; set; }
		public string? ProfileName { get; set; }		
		public int? Id_Case { get; set; }
	}
}