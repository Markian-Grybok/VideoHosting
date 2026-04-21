import client from "./client";
import type {
  Course, CourseDetails, LessonDetails,
  CreateCourseRequest, CreateLessonRequest, UpdateLessonRequest
} from "../types";

export const contentApi = {
  // Courses
  getCourses: () =>
    client.get<{ items: Course[]; totalCount: number }>("/api/Courses")
      .then(r => r.data),

  getCourse: (id: string) =>
    client.get<CourseDetails>(`/api/courses/${id}`)
      .then(r => r.data),

  createCourse: (data: CreateCourseRequest) =>
    client.post<Course>("/api/courses", data)
      .then(r => r.data),

  updateCourse: (id: string, data: CreateCourseRequest) =>
    client.put<Course>(`/api/courses/${id}`, data)
      .then(r => r.data),

  deleteCourse: (id: string) =>
    client.delete(`/api/courses/${id}`),

  // Lessons
  getLessons: (courseId?: string) =>
    client.get<LessonDetails[]>("/api/lessons", {
      params: courseId ? { courseId } : undefined
    }).then(r => r.data),

  getLesson: (id: string) =>
    client.get<LessonDetails>(`/api/lessons/${id}`)
      .then(r => r.data),

  createLesson: (data: CreateLessonRequest) =>
    client.post<LessonDetails>("/api/lessons", data)
      .then(r => r.data),

  updateLesson: (id: string, data: UpdateLessonRequest) =>
    client.put<LessonDetails>(`/api/lessons/${id}`, data)
      .then(r => r.data),

  deleteLesson: (id: string) =>
    client.delete(`/api/lessons/${id}`),
};
