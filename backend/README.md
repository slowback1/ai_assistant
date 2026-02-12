# .NET Starter Kit

A starter kit for .NET projects with a clean architecture approach, including multiple data persistence options (
InMemory and FileData), comprehensive testing, and automated build tasks.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Task](https://taskfile.dev/installation/) (for automated development tasks)

## Quick Start

1. **Initial Setup**
   ```bash
   task setup
   ```
   This will restore NuGet packages and create the required `appsettings.Development.json` file.

2. **Build the Solution**
   ```bash
   task build
   ```

3. **Run Tests**
   ```bash
   task test
   ```

4. **Run the API**
   ```bash
   task run
   # or for development environment with enhanced logging:
   task run-dev
   ```

## Available Tasks

You can see all available tasks by running:

```bash
task --list
```

### Core Tasks

- **`task setup`** - Initial project setup (restore packages + create development appsettings)
- **`task build`** - Build the entire solution
- **`task test`** - Run all unit tests
- **`task run`** - Run the WebAPI project
- **`task run-dev`** - Run the WebAPI with Development environment
- **`task clean`** - Clean build artifacts
- **`task lint`** - Run code analysis
- **`task lint-fix`** - Fix code style issues automatically

### Additional Tasks

- **`task restore`** - Restore NuGet packages only
- **`task watch`** - Run the API with file watching (auto-restart on changes)
- **`task help`** - Show available tasks

## Project Structure

```
├── Common/                     # Shared models and interfaces
├── Common.Tests/               # Tests for Common project
├── FileData/                   # File-based data persistence layer
├── FileData.Tests/             # Tests for FileData project
├── InMemory/                   # In-memory data persistence layer
├── InMemory.Tests/             # Tests for InMemory project
├── Logic/                      # Business logic layer
├── Logic.Tests/                # Tests for Logic project
├── WebAPI/                     # ASP.NET Core Web API
├── WebAPI.Integration.Tests/   # Integration tests for WebAPI
├── DotNetStarterKit.sln        # Solution file
└── Taskfile.yml                # Task automation configuration
```

## Development Workflow

1. **Start a new feature**: Run `task setup` to ensure you have the latest dependencies
2. **Development**: Use `task watch` for auto-restarting development
3. **Testing**: Run `task test` frequently to ensure your changes don't break existing functionality
4. **Build verification**: Run `task build` before committing changes

## Configuration

The application supports different data persistence modes via `appsettings.json` configuration:

- **InMemory**: Data stored in memory (default for development)
- **FileData**: Data persisted to local JSON files

Example configuration in `appsettings.Development.json`:

```json
{
  "CrudFactory": {
    "Implementation": "InMemory"  // or "FileData"
  },
  "FileData": {
    "Directory": "data"  // Directory for FileData storage
  }
}
```

## API Documentation

When running in Development mode, Swagger UI is available at: `http://localhost:5272/swagger`

## Memory System for Story Generation

The AI Assistant includes an advanced memory system that maintains narrative continuity over long story generation sessions.

### Features

- **Automatic Memory Extraction**: After each story is generated, the system extracts significant facts, relationships, goals, and events
- **Semantic Deduplication**: Prevents duplicate memories using AI-powered similarity detection
- **Long-term & Short-term Memory**: Supports both temporary (recent events) and permanent (character facts) memories
- **Memory Retrieval**: Automatically includes relevant memories in story prompts for continuity
- **Configurable Thresholds**: Control confidence levels, similarity thresholds, and memory limits

### Configuration

Memory system settings can be configured in `appsettings.json`:

```json
{
  "Memory": {
    "EnableMemoryExtraction": false,  // Set to true to enable memory system
    "ConfidenceThreshold": 0.7,       // Minimum confidence (0.0-1.0) to accept a memory
    "SimilarityThreshold": 0.85,      // Similarity threshold (0.0-1.0) for duplicate detection
    "MaxMemoriesPerPrompt": 10,       // Maximum number of memories to include in prompts
    "ShortTermMemoryCount": 5,        // Number of recent events to include in context
    "MemoryTtlDays": 180              // Days to retain memories before archiving
  }
}
```

### API Endpoints

#### Generate Story with Memory Support
```
POST /Story/Generate?sessionId=optional-session-id
```
Generates a new story segment and automatically extracts memories if enabled.

#### Retrieve Memories
```
GET /Story/Memories?personalityId=optional-personality-id
```
Returns all stored memories, optionally filtered by personality.

### Memory Data Model

Each memory record contains:
- **Summary**: Brief description of the fact/event
- **Type**: `ShortTerm` or `LongTerm`
- **PersonalityId**: Associated character/personality
- **SessionId**: Optional session identifier
- **SourceStoryEventId**: Link to the story event that created this memory
- **Tags**: Categorization tags
- **ConfidenceScore**: AI confidence in this memory (0.0-1.0)
- **CreatedAt**: Timestamp

### How It Works

1. **Story Generation**: The system loads relevant long-term memories and includes them in the prompt
2. **Memory Extraction**: After generating a story, a secondary AI prompt analyzes the output
3. **Candidate Evaluation**: Extracted memories are evaluated for confidence and deduplicated
4. **Storage**: Accepted memories are persisted via the configured CRUD storage backend

### Rollout Phases

The memory system is designed to be rolled out in phases:

- **Phase 0**: Behind feature flag (disabled by default)
- **Phase 1**: Shadow writes (stores but doesn't use memories)
- **Phase 2**: Read + Write for beta users
- **Phase 3**: General availability
- **Phase 4**: Enhanced features (embeddings, human moderation)

## Contributing

1. Make sure all tests pass: `task test`
2. Ensure the build succeeds: `task build`
3. Test the API runs correctly: `task run-dev`