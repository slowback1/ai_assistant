# GitHub Actions Workflows

## Docker Build and Push

The `docker-build-push.yml` workflow automatically builds and pushes Docker images for the backend and frontend when code is pushed to the `main` branch.

### Required GitHub Secrets

The following secrets must be configured in the GitHub repository settings (Settings → Secrets and variables → Actions):

1. **DOCKERHUB_USERNAME**: Your Docker Hub username
2. **DOCKERHUB_TOKEN**: Your Docker Hub access token (not your password)
   - Create a token at: https://hub.docker.com/settings/security
3. **DOCKERHUB_REPOSITORY_BACKEND**: Full Docker Hub repository name for the backend image
   - Format: `username/repository-name` (e.g., `myuser/my-backend`)
4. **DOCKERHUB_REPOSITORY_FRONTEND**: Full Docker Hub repository name for the frontend image
   - Format: `username/repository-name` (e.g., `myuser/my-frontend`)

### Image Tags

The workflow automatically generates the following tags for each image:
- `main` - The branch name
- `main-<git-sha>` - Branch name with the commit SHA
- `latest` - Only applied when pushing to the default branch (main)
