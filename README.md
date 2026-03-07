# AI Assistant - For Future AI Agents

This README helps AI agents understand and work with this codebase. It provides context, patterns, and guidance specific to this project.

## Project Overview

**AI Assistant** is a full-stack web application that generates AI-powered stories with:
- **Story Generation** - AI-powered narrative generation with configurable models
- **Memory System** - Maintains narrative continuity over long story sessions
- **Character Personalities** - Create and manage AI character personas
- **Donetick Integration** - Task management (feature-flag controlled)
- **Weather Widget** - Weather information display

### Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 8.0 Web API |
| Frontend | SvelteKit + Svelte 5 + TypeScript |
| Testing | xUnit (.NET), Vitest (Frontend), Cypress (E2E) |
| Build | Task (taskfile.dev), Vite |

---

## Architecture Overview

### Backend Structure (`/backend/`)

```
backend/
├── WebAPI/                 # Entry point (port 5272)
├── Logic/                  # Business logic
├── AIChat/                 # AI story generation
├── CharacterPersonalities/ # Persona management
├── Common/                 # Shared models/interfaces
├── InMemory/               # In-memory storage
├── FileData/               # File-based storage (JSON)
├── Donetick/               # Task API integration
├── Weather/                # Weather API integration
└── DotNetStarterKit.sln   # Solution file
```

**Key Pattern**: Uses dependency injection with interface-based abstractions. Storage implementations (`InMemory`, `FileData`) are swappable via configuration.

### Frontend Structure (`/frontend/src/`)

```
frontend/src/
├── routes/              # SvelteKit file-based routing
├── lib/
│   ├── api/             # API clients
│   ├── bus/             # MessageBus (pub/sub)
│   ├── components/      # App components
│   ├── services/        # Config, Feature Flags, Theme
│   └── ui/              # Reusable UI components
```

---

## Key Patterns & Conventions

### 1. Storage Abstraction

The backend uses a factory pattern for data persistence. To add a new storage type:

1. Create a new project (e.g., `SqlData/`)
2. Implement `ICrudFactory<T>` interfaces from `Common/`
3. Register in `Program.cs` with the configuration key

Configuration in `appsettings.json`:
```json
{
  "CrudFactory": {
    "Implementation": "InMemory"  // or "FileData"
  }
}
```

### 2. Feature Flags

Frontend uses a subscriber-based feature flag system. Feature flags are defined in `src/lib/services/FeatureFlag/FeatureFlags.ts` and loaded from runtime config.

To add a new feature flag:
1. Add to `FeatureFlags.ts`
2. Use in component via `FeatureFlagService.subscribeToFeature()` or `<FeatureToggle>` component

### 3. MessageBus

Framework-agnostic pub/sub system. Used for cross-component communication. Persists to `localStorage` by default.

### 4. API Middleware

The `baseAPI` class uses middleware for request/response transformation. Add custom middleware by:
1. Implementing `IRequestMiddleware`
2. Adding via `addMiddleware()` method

### 5. Memory System

Advanced system for story continuity. Key components:
- **Memory Extraction**: Analyzes generated stories for facts/relationships
- **Deduplication**: Uses AI similarity detection to avoid duplicates
- **Storage**: Supports short-term (recent events) and long-term (character facts) memories

Enabled via `appsettings.json`:
```json
{
  "Memory": {
    "EnableMemoryExtraction": false,
    "ConfidenceThreshold": 0.7,
    "SimilarityThreshold": 0.85
  }
}
```

---

## Common Development Tasks

### Running the Application

**Backend:**
```bash
cd backend
task setup    # First time: restore packages + create dev config
task run-dev  # Runs on http://localhost:5272
task watch    # Auto-restart on file changes
```

**Frontend:**
```bash
cd frontend
npm run dev   # Runs on http://localhost:5173
```

### Testing

**Backend:**
```bash
cd backend
task test     # Run all xUnit tests
task lint     # Run code analysis
```

**Frontend:**
```bash
cd frontend
npm run test           # Vitest unit tests
npm run cypress:open   # Cypress E2E tests
```

### Building

**Backend:**
```bash
cd backend
task build
```

**Frontend:**
```bash
cd frontend
npm run build
```

---

## Important Files Reference

| File | Purpose |
|------|---------|
| `backend/WebAPI/Program.cs` | DI container setup, middleware registration |
| `backend/WebAPI/appsettings.json` | Main configuration (AI, Memory, Donetick, Weather) |
| `frontend/src/routes/+layout.svelte` | App initialization (MessageBus, Config, FeatureFlags) |
| `frontend/static/config/config.example.json` | Runtime config template |
| `backend/Taskfile.yml` | All backend automation tasks |

### API Endpoints (Backend)

- `GET/POST /Story` - Story CRUD
- `POST /Story/Generate?sessionId=` - Generate story (with memory support)
- `GET /Story/Memories?personalityId=` - Retrieve memories
- `GET/POST /Personality` - Character personalities
- `GET /Weather` - Weather info (if configured)

Swagger available at `http://localhost:5272/swagger` (Dev mode)

---

## Configuration Guide

### Backend (`appsettings.json`)

```json
{
  "AI": {
    "Url": "https://api.example.com",
    "ApiKey": "your-key",
    "Model": "model-name"
  },
  "Memory": {
    "EnableMemoryExtraction": false,
    "ConfidenceThreshold": 0.7,
    "SimilarityThreshold": 0.85,
    "MaxMemoriesPerPrompt": 10
  },
  "Donetick": {
    "ApiKey": "...",
    "InstanceUrl": "..."
  },
  "Weather": {
    "ApiKey": "...",
    "ZipCode": "..."
  }
}
```

### Frontend (`config/config.json`)

```json
{
  "baseUrl": "http://localhost:5272",
  "featureFlags": {
    "DONETICK_INTEGRATION": false
  }
}
```

---

## Adding New Features (Quick Guide)

### Backend

1. **New API Endpoint**: Add controller in `WebAPI/Controllers/`
2. **New Business Logic**: Add service class in `Logic/`
3. **New Data Model**: Add model in `Common/Models/`
4. **New Configuration**: Add section to `appsettings.json`

### Frontend

1. **New Page**: Create route in `src/routes/`
2. **New Component**: Add to `src/lib/components/`
3. **New API Client**: Extend `baseAPI` in `src/lib/api/`
4. **New Feature Flag**: Add to `FeatureFlags.ts` and config

---

## Troubleshooting

- **API not responding**: Check backend is running on port 5272
- **Config not loading**: Ensure `config.json` exists in `frontend/static/config/`
- **Memory system not working**: Verify `EnableMemoryExtraction` is `true` in appsettings
- **Build fails**: Run `task setup` or `npm install` first
- **CORS errors**: Check backend CORS policy in `Program.cs`

---

## CI/CD

- **GitHub Actions**: See `.github/workflows/`
- **Docker**: Dockerfiles in `backend/WebAPI/` and `frontend/docker/`
- **Tests**: Run on every push to main and PRs
