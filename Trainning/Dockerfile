FROM python:3.11-slim

WORKDIR /app

# Install build dependencies, then pip packages, then clean up build tools
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    curl \
    && python -m pip install --upgrade pip setuptools \
    && rm -rf /var/lib/apt/lists/*

# Copy requirements and install Python packages
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt \
    && apt-get remove -y build-essential \
    && apt-get autoremove -y \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy application code (excluding files in .dockerignore)
COPY . /app

EXPOSE 8000

# Healthcheck
HEALTHCHECK --interval=20s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8000/health || exit 1

CMD ["uvicorn", "api:app", "--host", "0.0.0.0", "--port", "8000"]
