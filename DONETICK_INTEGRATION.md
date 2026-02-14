# Donetick Integration

This document describes the Donetick API integration implementation for the AI Assistant dashboard.

## Overview

The Donetick integration allows users to view and complete overdue chores from their self-hosted Donetick instance directly from the AI Assistant dashboard. The feature is hidden behind a feature flag and can be easily enabled or disabled.

## Architecture

### Backend

**New Class Library: `Donetick`**
- `DonetickConfig.cs` - Configuration model for API key and instance URL
- `ChoreDto.cs` - Data transfer object for chore information
- `DonetickClient.cs` - HTTP client for communicating with Donetick API
- `DonetickService.cs` - Business logic layer that filters overdue chores

**Controller: `DonetickController`**
- `GET /Donetick/OverdueChores` - Returns list of overdue chores
- `POST /Donetick/CompleteChore/{choreId}` - Marks a chore as complete

**Configuration:**
The backend reads Donetick configuration from `appsettings.json`:
```json
{
  "Donetick": {
    "ApiKey": "your-api-key",
    "InstanceUrl": "https://your-donetick-instance.com"
  }
}
```

The service is only registered if both ApiKey and InstanceUrl are configured.

### Frontend

**API Service: `DonetickApi.ts`**
- Extends `BaseApi` for consistent HTTP communication
- Methods: `GetOverdueChores()`, `CompleteChore(choreId)`

**Component: `DonetickTaskList.svelte`**
- Displays list of overdue chores
- Provides "Complete" button for each chore
- Shows loading states and error messages
- Polls for updates every 5 minutes
- Integrates with toast notification system

**Dashboard Integration:**
The component is conditionally rendered in the dashboard based on the `DONETICK_ENABLED` feature flag.

**Feature Flag:**
Add to `static/config/config.json`:
```json
{
  "featureFlags": [
    {
      "name": "DONETICK_ENABLED",
      "isEnabled": true
    }
  ]
}
```

## Usage

### Backend Setup

1. Configure your Donetick instance in `appsettings.json`:
```json
{
  "Donetick": {
    "ApiKey": "your-secret-key",
    "InstanceUrl": "https://donetick.example.com"
  }
}
```

2. The backend will automatically register the Donetick service if both values are provided.

### Frontend Setup

1. Enable the feature flag in `static/config/config.json`:
```json
{
  "featureFlags": [
    {
      "name": "DONETICK_ENABLED",
      "isEnabled": true
    }
  ]
}
```

2. The Donetick task list will now appear on the dashboard page.

## API Details

### Donetick API Endpoints Used

**Get All Chores:**
- Method: `GET`
- Endpoint: `/chore`
- Headers: `secretkey: {ApiKey}`
- Response: Array of chore objects with `id`, `name`, and `nextDueDate`

**Complete Chore:**
- Method: `POST`
- Endpoint: `/chore/{id}/complete`
- Headers: `secretkey: {ApiKey}`
- Response: HTTP status (200 = success)

## Testing

Unit tests are included for:
- `DonetickClient` - Tests API communication
- `DonetickService` - Tests business logic for filtering overdue chores

Run tests:
```bash
cd backend
dotnet test Donetick.Tests/Donetick.Tests.csproj
```

## Security

- API keys are stored in backend configuration only
- Frontend never has access to API keys
- Backend acts as a proxy for all Donetick API calls
- Feature flag allows easy disable if needed
