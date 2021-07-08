namespace Bonanza.Storage.Benchmark.Logging
{
	public class LogCorrelation
	{
		public LogCorrelation ParentCorrelation { get; }
		public string CorrelationId { get; }

		public LogCorrelation(string correlationId, LogCorrelation parentCorrelation)
		{
			this.CorrelationId = correlationId;
			this.ParentCorrelation = parentCorrelation;
		}

		public string GetCurrentAndParentCorrelation()
		{
			if (ParentCorrelation == null)
			{
				return string.Empty;
			}
			else
			{
				return $"{CorrelationId}--{ParentCorrelation.GetCurrentAndParentCorrelation()}";
			}
		}
	}
}
