# SurveyApp - Advanced Survey Management System

A comprehensive, feature-rich survey management application built with .NET 9.0, MongoDB, and modern web technologies. This application provides a complete solution for creating, managing, and analyzing surveys with advanced features including AI-powered question generation, role-based access control, and comprehensive activity logging.

## 🚀 Features

### Core Survey Management
- **Survey Creation & Management**: Create, edit, and manage surveys with rich configuration options
- **Question Types**: Support for multiple choice, text, rating, date, and custom question types
- **Answer Collection**: Comprehensive answer collection and validation system
- **Survey Analytics**: Built-in analytics and reporting capabilities

### Advanced Features
- **AI-Powered Survey Generation**: Generate complete surveys using Google AI (Gemini)
- **Smart Question Suggestions**: AI-driven question recommendations and improvements
- **Sentiment Analysis**: Analyze survey responses for emotional tone and sentiment
- **Activity Logging**: Comprehensive audit trail for all user activities
- **Role-Based Access Control**: Admin, Owner, and User roles with different permissions
- **Third-Party Integrations**: Google Auth, Trello, and Jira integration support

### User Management
- **Authentication & Authorization**: JWT-based authentication with role management
- **User Profiles**: Detailed user profile management with social media integration
- **User Settings**: Customizable user preferences and notification settings
- **Address Management**: Hierarchical address system (City → District → Township → Neighborhood)

### Calendar & Events
- **Event Management**: Full calendar functionality with event creation and management
- **Date Range Filtering**: Filter events by date ranges
- **Event Categories**: Organize events with colors and categories

## 🏗️ Architecture

### Technology Stack
- **Backend**: .NET 9.0 Web API
- **Database**: MongoDB with comprehensive indexing
- **Authentication**: JWT Bearer tokens
- **AI Integration**: Google AI (Gemini) for intelligent features
- **Third-Party APIs**: Google OAuth, Trello API, Jira API
- **File Storage**: Local file system with upload management

### Project Structure
The application follows a clean architecture pattern with clear separation of concerns across multiple layers including Controllers, Models, Services, Infrastructure, and dedicated API layers for better maintainability and scalability.

## 📋 Prerequisites

- .NET 9.0 SDK
- MongoDB (Local or Cloud)
- Google AI API Key (for AI features)
- Google OAuth Credentials (for Google Auth)
- Trello API Credentials (optional)
- Jira API Credentials (optional)

## 🛠️ Installation & Setup

### 1. Clone the Repository
Clone the repository to your local machine and navigate to the project directory.

### 2. Configure MongoDB
Update the appsettings.json file with your MongoDB connection string and database name.

### 3. Configure API Keys
Configure your API credentials in appsettings.json including JWT settings, Google AI API key, and Google OAuth credentials.

### 4. Install Dependencies
Restore all NuGet packages using the dotnet restore command.

### 5. Run the Application
Start the application using the dotnet run command. The application will be available on the configured port.

## 🗄️ Database Setup

### Initialize Roles
Set up the role system in MongoDB with three default roles: admin, owner, and user. Each role has specific permissions and access levels within the application.

### Create Admin User
Create an admin user account with the provided credentials including name, email, and hashed password. The admin user will have full system access and management capabilities.

## 🔐 Authentication & Authorization

### Role System
- **Admin (ID: 1)**: System administrator with full access
- **Owner (ID: 2)**: Survey creators with management permissions
- **User (ID: 3)**: Regular users who can participate in surveys

### Authentication Endpoints
- `POST /api/Auth/register` - User registration
- `POST /api/Auth/login` - User login
- `POST /api/Auth/admin-login` - Admin login
- `POST /api/Auth/logout` - User logout

### JWT Token Usage
Include the JWT token in the Authorization header for all authenticated requests. The token should be prefixed with "Bearer" and included in the request headers.

## 📊 API Endpoints

### Survey Management
- `GET /api/Surveys` - Get all surveys
- `POST /api/Surveys` - Create new survey
- `PUT /api/Surveys/{id}` - Update survey
- `DELETE /api/Surveys/{id}` - Delete survey
- `POST /api/Surveys/{id}/complete` - Complete survey

### Question Management
- `GET /api/Questions` - Get all questions
- `POST /api/Questions` - Create question
- `PUT /api/Questions/{id}` - Update question
- `DELETE /api/Questions/{id}` - Delete question

### User Management
- `GET /api/Users` - Get all users
- `GET /api/Users/{id}` - Get user by ID
- `PUT /api/Users/{id}` - Update user
- `PUT /api/Users/change-password` - Change password

### AI Features
- `POST /api/ai/analyze-survey/{surveyId}` - Analyze survey results
- `POST /api/ai/sentiment-analysis` - Analyze sentiment
- `POST /api/ai/generate-questions` - Generate smart questions
- `POST /api/ai/generate-complete-survey` - Generate complete survey
- `GET /api/ai/insights/{surveyId}` - Get survey insights

### Activity Logging
- `GET /api/ActivityLog` - Get activity logs
- `GET /api/ActivityLog/user/{userId}` - Get user logs
- `GET /api/ActivityLog/type/{activityType}` - Get logs by type

### Address Management
- `GET /api/Address/cities` - Get all cities
- `GET /api/Address/districts/{cityName}` - Get districts by city
- `GET /api/Address/district-township-towns/{cityName}/{districtName}` - Get townships
- `GET /api/Address/neighbourhoods/{cityName}/{districtName}/{townshipName}` - Get neighborhoods

## 🤖 AI Features

### Survey Analysis
The application includes comprehensive AI-powered features:

- **Survey Result Analysis**: Analyze survey responses with AI insights
- **Sentiment Analysis**: Determine emotional tone of responses
- **Smart Question Generation**: AI-generated question suggestions
- **Complete Survey Generation**: Generate entire surveys from descriptions
- **Text Improvement**: Enhance question clarity and effectiveness
- **Keyword Extraction**: Extract key themes from responses
- **Multi-language Support**: Translate surveys and responses

### AI Usage Examples

#### Generate Complete Survey
Use the AI endpoint to generate complete surveys from simple descriptions. Provide a description of the survey type and purpose, and the AI will create a comprehensive survey with relevant questions and structure.

#### Analyze Survey Results
Leverage AI-powered analysis to gain insights from survey responses. The system can perform comprehensive analysis including sentiment analysis, trend identification, and actionable insights generation.

## 📅 Event Management

### Calendar Features
- Create, update, and delete events
- Date range filtering
- Event categorization with colors
- All-day event support
- Location and description management

### Event API Examples
The event management system allows you to create, update, and delete calendar events with comprehensive details including title, description, date and time information, location, and color coding for better organization.

## 📱 Frontend Integration

### React/JavaScript Examples

#### Authentication Hook
Implement a custom authentication hook that manages user state, token storage, and login functionality. The hook handles API communication with the authentication endpoints and maintains user session state.

#### Survey Management Hook
Create a survey management hook that provides functionality for creating, updating, and managing surveys. The hook handles loading states, API communication, and state management for survey-related operations.

## 🔧 Configuration

### Environment Variables
The application supports multiple configuration sources:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides

### CORS Configuration
CORS is configured to allow all origins in development. For production, update the CORS policy in `Program.cs`.

### File Upload Configuration
- Maximum file size: 10MB
- Supported formats: Images (JPG, PNG, GIF)
- Upload directory: `wwwroot/uploads/`

## 📈 Monitoring & Logging

### Activity Logging
The application includes comprehensive activity logging:
- User login/logout events
- Survey creation and modification
- Error tracking and debugging
- Performance monitoring
- Security event logging

### Log Types
- `login_success` / `login_failed`
- `survey_created` / `survey_creation_failed`
- `survey_updated` / `survey_update_failed`
- `survey_completed` / `survey_completion_failed`
- `question_created`
- `answer_submitted`

## 🚀 Deployment

### Docker Support
The application includes Docker configuration for easy deployment and containerization. Use Docker commands to build the application image and run it with Docker Compose for a complete containerized environment.

### Production Considerations
- Update CORS policy for production domains
- Configure proper MongoDB connection strings
- Set up SSL certificates
- Configure logging levels
- Set up monitoring and alerting

## 🧪 Testing

### Test Endpoints
- Activity Log Test: `http://localhost:5000/test-activity-logs.html`
- File Upload Test: `http://localhost:5000/test-upload.html`

### Manual Testing
Test the application functionality using HTTP client tools or API testing platforms. Verify authentication endpoints, survey creation and management, user operations, and AI features to ensure all components are working correctly.

## 🤝 Contributing

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new features
5. Update documentation
6. Submit a pull request

### Code Standards
- Follow C# coding conventions
- Use async/await for I/O operations
- Include comprehensive error handling
- Write unit tests for new features
- Update API documentation

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the documentation in the `README` files
- Review the API documentation in Swagger UI

## 🔮 Roadmap

### Planned Features
- [ ] Real-time collaboration on surveys
- [ ] Advanced analytics dashboard
- [ ] Mobile application
- [ ] Webhook support for integrations
- [ ] Advanced reporting with charts
- [ ] Survey templates library
- [ ] Multi-language survey support
- [ ] Advanced user permissions system

### Performance Improvements
- [ ] Caching layer implementation
- [ ] Database query optimization
- [ ] Background job processing
- [ ] CDN integration for file uploads
