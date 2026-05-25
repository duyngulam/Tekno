using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tekno.Infrastructure.Seeding
{
    public class TrainingPipelineRunner
    {
        private readonly ILogger<TrainingPipelineRunner> _logger;

        public TrainingPipelineRunner(ILogger<TrainingPipelineRunner> logger)
        {
            _logger = logger;
        }

        public async Task<PipelineRunResult> RunAsync(string pythonExe, string workingDir, string args)
        {
            var output = new StringBuilder();
            var error = new StringBuilder();

            var psi = new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = args,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = new Process { StartInfo = psi };
            process.OutputDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) error.AppendLine(e.Data); };

            _logger.LogInformation("Running pipeline: {Exe} {Args} (cwd={Cwd})", pythonExe, args, workingDir);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            return new PipelineRunResult
            {
                ExitCode = process.ExitCode,
                Stdout = output.ToString(),
                Stderr = error.ToString(),
            };
        }
    }

    public class PipelineRunResult
    {
        public int ExitCode { get; set; }
        public string Stdout { get; set; } = string.Empty;
        public string Stderr { get; set; } = string.Empty;
    }
}
