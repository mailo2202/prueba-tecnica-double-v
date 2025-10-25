# Use Ruby 3.2 as base image
FROM ruby:3.2-slim

# Install system dependencies
RUN apt-get update -qq && apt-get install -y \
    build-essential \
    curl \
    git \
    libyaml-dev \
    pkg-config \
    wget \
    unzip \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy Gemfile and Gemfile.lock
COPY Gemfile Gemfile.lock ./

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

# Run database migrations (MongoDB)
RUN bundle exec rails db:create db:migrate RAILS_ENV=production

# Precompile assets (if any)
RUN bundle exec rails assets:precompile

# Start the application
CMD ["bundle", "exec", "rails", "server", "-b", "0.0.0.0", "-p", "3001"]
