// MongoDB initialization script for Audit Service
// This script creates collections and indexes

// Connect to audit database
use auditoria_db;

// Create audit events collection
db.createCollection("audit_events");

// Create indexes for query optimization
db.audit_events.createIndex(
  { "entity": 1, "entity_id": 1 },
  { "name": "idx_entity_entity_id" }
);

db.audit_events.createIndex(
  { "service": 1 },
  { "name": "idx_service" }
);

db.audit_events.createIndex(
  { "timestamp": 1 },
  { "name": "idx_timestamp" }
);

db.audit_events.createIndex(
  { "event_type": 1 },
  { "name": "idx_event_type" }
);

db.audit_events.createIndex(
  { "user_id": 1 },
  { "name": "idx_user_id" }
);

// Compound index for frequent queries
db.audit_events.createIndex(
  { "service": 1, "timestamp": -1 },
  { "name": "idx_service_timestamp_desc" }
);

// Insert test data
db.audit_events.insertMany([
  {
    "event_type": "CREATE",
    "entity": "Client",
    "entity_id": 1,
    "details": "Client created: Empresa ABC S.A.S",
    "service": "ClientsService",
    "timestamp": new Date(),
    "user_id": 1,
    "ip_address": "192.168.1.100",
    "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "metadata": {
      "version": "1.0",
      "source": "api",
      "request_id": "req-001"
    },
    "created_at": new Date(),
    "updated_at": new Date()
  },
  {
    "event_type": "CREATE",
    "entity": "Invoice",
    "entity_id": 1,
    "details": "Invoice created: FAC-20240115001, Amount: $150,000.00",
    "service": "FacturasService",
    "timestamp": new Date(),
    "user_id": 1,
    "ip_address": "192.168.1.101",
    "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "metadata": {
      "version": "1.0",
      "source": "api",
      "request_id": "req-002"
    },
    "created_at": new Date(),
    "updated_at": new Date()
  },
  {
    "event_type": "READ",
    "entity": "Client",
    "entity_id": 1,
    "details": "Client query by ID",
    "service": "ClientsService",
    "timestamp": new Date(),
    "user_id": 2,
    "ip_address": "192.168.1.102",
    "user_agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
    "metadata": {
      "version": "1.0",
      "source": "api",
      "request_id": "req-003"
    },
    "created_at": new Date(),
    "updated_at": new Date()
  },
  {
    "event_type": "UPDATE",
    "entity": "Client",
    "entity_id": 2,
    "details": "Client updated: name, email",
    "service": "ClientsService",
    "timestamp": new Date(),
    "user_id": 1,
    "ip_address": "192.168.1.103",
    "user_agent": "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36",
    "metadata": {
      "version": "1.0",
      "source": "api",
      "request_id": "req-004"
    },
    "created_at": new Date(),
    "updated_at": new Date()
  },
  {
    "event_type": "ERROR",
    "entity": "Invoice",
    "entity_id": 0,
    "details": "Error creating invoice: Client with ID 999 does not exist",
    "service": "FacturasService",
    "timestamp": new Date(),
    "user_id": 1,
    "ip_address": "192.168.1.104",
    "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "metadata": {
      "version": "1.0",
      "source": "api",
      "request_id": "req-005",
      "error_code": "CLIENT_NOT_FOUND"
    },
    "created_at": new Date(),
    "updated_at": new Date()
  }
]);

// Create audit statistics collection
db.createCollection("audit_stats");

db.audit_stats.insertOne({
  "total_events": 5,
  "events_by_service": {
    "ClientsService": 3,
    "FacturasService": 2
  },
  "events_by_type": {
    "CREATE": 2,
    "READ": 1,
    "UPDATE": 1,
    "ERROR": 1
  },
  "last_updated": new Date()
});

// Create system configuration collection
db.createCollection("system_config");

db.system_config.insertOne({
  "configuration": {
    "retention_days": 365,
    "max_events_per_query": 1000,
    "enable_real_time_alerts": true,
    "alert_thresholds": {
      "error_rate": 0.05,
      "response_time_ms": 5000
    }
  },
  "version": "1.0.0",
  "created_at": new Date(),
  "updated_at": new Date()
});

// Create application-specific user
db.createUser({
  user: "audit_user",
  pwd: "audit_pass",
  roles: [
    {
      role: "readWrite",
      db: "auditoria_db"
    }
  ]
});

// Create views for common queries (MongoDB 3.4+)
db.createView(
  "recent_events",
  "audit_events",
  [
    {
      $match: {
        timestamp: {
          $gte: new Date(Date.now() - 24 * 60 * 60 * 1000) // Last 24 hours
        }
      }
    },
    {
      $sort: {
        timestamp: -1
      }
    },
    {
      $limit: 100
    }
  ]
);

db.createView(
  "events_by_service",
  "audit_events",
  [
    {
      $group: {
        _id: "$service",
        total: { $sum: 1 },
        last_event: { $max: "$timestamp" }
      }
    },
    {
      $sort: {
        total: -1
      }
    }
  ]
);

// Create function to clean old events
db.system.js.save({
  _id: "cleanOldEvents",
  value: function(retentionDays) {
    var cutoffDate = new Date(Date.now() - retentionDays * 24 * 60 * 60 * 1000);
    var result = db.audit_events.deleteMany({
      timestamp: { $lt: cutoffDate }
    });
    return {
      deleted: result.deletedCount,
      cutoffDate: cutoffDate
    };
  }
});

print("MongoDB database initialized successfully for Audit Service");
print("Collections created: audit_events, audit_stats, system_config");
print("Indexes created: 6 indexes for query optimization");
print("Test data inserted: 5 audit events");
print("Views created: recent_events, events_by_service");
print("User created: audit_user");
