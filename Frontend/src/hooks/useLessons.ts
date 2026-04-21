import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { contentApi } from "../api/contentApi";
import type { CreateLessonRequest, UpdateLessonRequest } from "../types";

export function useLessons(courseId?: string) {
  return useQuery({
    queryKey: ["lessons", courseId],
    queryFn: () => contentApi.getLessons(courseId),
  });
}

export function useLesson(id: string) {
  return useQuery({
    queryKey: ["lessons", "detail", id],
    queryFn: () => contentApi.getLesson(id),
    enabled: !!id,
  });
}

export function useCreateLesson() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateLessonRequest) => contentApi.createLesson(data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["courses", data.courseId] });
      queryClient.invalidateQueries({ queryKey: ["lessons"] });
    },
  });
}

export function useUpdateLesson() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateLessonRequest }) =>
      contentApi.updateLesson(id, data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["courses", data.courseId] });
      queryClient.invalidateQueries({ queryKey: ["lessons"] });
      queryClient.invalidateQueries({ queryKey: ["lessons", "detail", data.id] });
    },
  });
}

export function useDeleteLesson() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: contentApi.deleteLesson,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["courses"] });
      queryClient.invalidateQueries({ queryKey: ["lessons"] });
    },
  });
}
