# AI Project Frontend Integration Guide

This guide provides the necessary information to integrate the Next.js frontend with the .NET backend API. It includes API endpoints, request/response DTOs, and TypeScript type definitions.

## Base API URL
All API endpoints are prefixed with:
`http://localhost:5000/api/v1`

---

## 1. Chat Module

### **Start a Chat Session**
- **Endpoint**: `POST /chat`
- **Request DTO**:
```typescript
interface StartChatRequest {
  userId: string; // GUID
  title: string;
  aiModelId: string; // e.g., "gpt-4"
}
```
- **Response**: `{ id: string }`

### **Send a Message**
- **Endpoint**: `POST /chat/send-message`
- **Request DTO**:
```typescript
interface SendMessageRequest {
  sessionId: string; // GUID
  content: string;
  sender: string; // "User" or "AI"
  tokenUsed: number;
}
```
- **Response**: `{ id: string }`

### **Get Chat History**
- **Endpoint**: `GET /chat/history/{userId}`
- **Response**:
```typescript
interface ChatHistoryResponse {
  chatDtos: Array<{
    id: string;
    userId: string;
    title: string;
    summary: string;
    aiModelId: string;
    sessionStatus: string;
    lastSentAt: string; // ISO Date string
    totalTokens: number;
  }>;
}
```

---

## 2. Image Captioning Module

### **Upload Image**
- **Endpoint**: `POST /image/upload`
- **Request DTO**:
```typescript
interface UploadImageRequest {
  userId: string;
  imageUrl: string;
  fileName: string;
  width: number;
  height: number;
  size: number;
  format: string;
}
```
- **Response**: `{ id: string }`

### **Generate Caption**
- **Endpoint**: `POST /image/generate-caption`
- **Request DTO**:
```typescript
interface GenerateCaptionRequest {
  imageId: string;
  captionText: string;
  confidence: number;
  language: string;
}
```

---

## 3. Learning Assistant Module

### **Generate Lesson**
- **Endpoint**: `POST /assistant/lesson`
- **Request DTO**:
```typescript
interface GenerateLessonRequest {
  profileId: string;
  title: string;
  content: string;
}
```

### **Submit Quiz**
- **Endpoint**: `POST /assistant/quiz/submit`
- **Request DTO**:
```typescript
interface SubmitQuizeRequest {
  quizId: string;
  answer: string;
}
```

---

## 4. Meeting Module

### **Upload Meeting Audio**
- **Endpoint**: `POST /meeting/upload`
- **Request DTO**:
```typescript
interface UploadMeetingAudioRequest {
  organizerId: string;
  title: string;
  audioUrl: string;
}
```

### **Get Meeting Summary**
- **Endpoint**: `GET /meeting/{meetingId}/summary`
- **Response**:
```typescript
interface MeetingSummaryDto {
  id: string;
  title: string;
  summary: string;
  status: string;
  createdAt: string;
}
```

---

## 5. Resume Module

### **Upload Resume**
- **Endpoint**: `POST /resume/upload`
- **Request DTO**:
```typescript
interface UploadResumeRequest {
  userId: string;
  candidateName: string;
  resumeUrl: string;
  fileName: string;
}
```

### **Get Resume Analysis**
- **Endpoint**: `GET /resume/{resumeId}/analysis`
- **Response**:
```typescript
interface ResumeAnalysisDto {
  id: string;
  candidateName: string;
  summary: string;
  status: string;
  analyzedAt: string | null;
}
```

---

## 6. User Module

### **Get User Usage Summary**
- **Endpoint**: `GET /user/usage/{userId}`
- **Response**:
```typescript
interface UserUsageSummaryDto {
  id: string;
  period: string;
  tokenUsed: string;
  requestsCount: number;
}
```

### **Get User Activity**
- **Endpoint**: `GET /user/activity/{userId}`
- **Response**:
```typescript
interface UserActivityDto {
  id: string;
  module: string;
  action: string;
  resourceId: string;
  timeStamp: string;
}
```

---

## Authentication
All requests require a **Bearer Token** in the Authorization header.
The token should have the `ai-net` scope.

```javascript
headers: {
  'Authorization': `Bearer ${accessToken}`,
  'Content-Type': 'application/json'
}
```

## Error Handling
The API returns standard RFC 7807 **Problem Details** for errors (400, 401, 404, 500).
Check the `type` and `detail` fields in the response body.
