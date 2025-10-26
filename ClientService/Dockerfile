# Use Ruby 3.4.7 as base image (same as AuditService)
FROM ruby:3.4.7-slim

# Install system dependencies
RUN apt-get update -qq && apt-get install -y \
    build-essential \
    libpq-dev \
    curl \
    git \
    libyaml-dev \
    pkg-config \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy Gemfile and Gemfile.lock if it exists
COPY Gemfile* ./

# Install gems
RUN bundle install

# Copy application code
COPY . .

# Create necessary directories
RUN mkdir -p tmp/pids db

# Expose port
EXPOSE 3001

# Set environment variables
ENV RAILS_ENV=production
ENV RAILS_SERVE_STATIC_FILES=true
ENV RAILS_LOG_TO_STDOUT=true

# Make bin files executable
RUN chmod +x bin/rails bin/bundle

# Fix line endings (convert Windows line endings to Unix)
RUN sed -i 's/\r$//' bin/rails bin/bundle || true

# Precompile assets (if any) - se omite temporalmente para debug
# RUN bundle exec rails assets:precompile

# Start the application
CMD ["sh", "-c", "rm -f tmp/pids/server.pid && exec bundle exec rails server -b 0.0.0.0 -p 3001"]
