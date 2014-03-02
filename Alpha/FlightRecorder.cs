namespace Alpha
{
    using System;
    using Toolkit.IO;
    class FlightRecorder : GameComponent
    {
        private readonly CsvLogger _logger;
        public FlightRecorder(Game game, params ICsvLoggable[] items)
            : base(game, 10000)
        {
            _logger = new CsvLogger(@"Logs\FlightRecording_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv", 1, ';');
            _logger.Register(items);
        }

        public override void Update(double delta)
        {
            _logger.Log();
        }

        public override void Dispose()
        {
            _logger.Dispose();
        }
    }
}
