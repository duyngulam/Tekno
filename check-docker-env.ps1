# ================================================================
# Docker Environment Variable Diagnostics
# ================================================================
# Run this script to verify your .env file is configured correctly

Write-Host "?? Tekno API - Docker Environment Diagnostics" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check 1: Verify .env file exists
Write-Host "? Checking if .env file exists..." -ForegroundColor Yellow
if (Test-Path ".env") {
    Write-Host "  ? .env file found" -ForegroundColor Green
} else {
    Write-Host "  ? .env file NOT found!" -ForegroundColor Red
    Write-Host "  ?? Create it by running: cp .env.example .env" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Check 2: Verify Cloudinary URL is set
Write-Host "? Checking Cloudinary configuration..." -ForegroundColor Yellow
$envContent = Get-Content .env -Raw
if ($envContent -match 'CLOUDINARY_URL=cloudinary://') {
    Write-Host "  ? CLOUDINARY_URL is configured" -ForegroundColor Green
    
    # Extract and display (masked)
    if ($envContent -match 'CLOUDINARY_URL=(cloudinary://[^@]+@[\w]+)') {
        $url = $matches[1]
        # Mask the secret
        $masked = $url -replace '(cloudinary://\d+:)[^@]+(@)', '$1***MASKED***$2'
        Write-Host "  ?? Value: $masked" -ForegroundColor Gray
    }
} else {
    Write-Host "  ? CLOUDINARY_URL is NOT configured or has wrong format!" -ForegroundColor Red
    Write-Host "  ?? Expected format: CLOUDINARY_URL=cloudinary://API_KEY:API_SECRET@CLOUD_NAME" -ForegroundColor Yellow
    Write-Host "  ?? Get your credentials from: https://cloudinary.com/console" -ForegroundColor Cyan
}
Write-Host ""

# Check 3: Verify docker-compose can resolve variables
Write-Host "? Testing docker-compose configuration..." -ForegroundColor Yellow
try {
    $composeTest = docker-compose config 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? docker-compose config is valid" -ForegroundColor Green
        
        # Check if Cloudinary URL is in the resolved config
        if ($composeTest -match 'Cloudinary__Url') {
            Write-Host "  ? Cloudinary__Url found in docker-compose config" -ForegroundColor Green
        } else {
            Write-Host "  ??  Cloudinary__Url not found in docker-compose config" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ? docker-compose config has errors!" -ForegroundColor Red
        Write-Host $composeTest -ForegroundColor Red
    }
} catch {
    Write-Host "  ? docker-compose is not installed or not in PATH" -ForegroundColor Red
}
Write-Host ""

# Check 4: List all environment variables in .env
Write-Host "? Environment variables in .env file:" -ForegroundColor Yellow
$envLines = Get-Content .env | Where-Object { $_ -match '^[A-Z_]+=.+' -and $_ -notmatch '^#' }
foreach ($line in $envLines) {
    if ($line -match '^([^=]+)=(.+)$') {
        $key = $matches[1]
        $value = $matches[2]
        
        # Mask sensitive values
        if ($key -match 'PASSWORD|SECRET|KEY|URL') {
            $value = "***MASKED***"
        }
        
        Write-Host "  • $key = $value" -ForegroundColor Gray
    }
}
Write-Host ""

# Check 5: Verify .gitignore protects .env
Write-Host "? Checking .gitignore protection..." -ForegroundColor Yellow
if (Test-Path ".gitignore") {
    $gitignore = Get-Content .gitignore -Raw
    if ($gitignore -match '(^|\n)\.env(\s|$)') {
        Write-Host "  ? .env is in .gitignore (protected)" -ForegroundColor Green
    } else {
        Write-Host "  ??  .env might not be in .gitignore!" -ForegroundColor Yellow
        Write-Host "  ?? Add this line to .gitignore: .env" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ??  .gitignore not found" -ForegroundColor Yellow
}
Write-Host ""

# Summary
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "?? Summary" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "? If all checks passed, run:" -ForegroundColor Green
Write-Host "   docker-compose down" -ForegroundColor White
Write-Host "   docker-compose up --build" -ForegroundColor White
Write-Host ""
Write-Host "? If checks failed, fix the issues and run this script again." -ForegroundColor Red
Write-Host ""
Write-Host "?? Need help? Check: SECRETS_MANAGEMENT.md" -ForegroundColor Cyan
