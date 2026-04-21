import React, { useState } from "react";
import { useCreateCourse } from "../../hooks/useCourses";
import Button from "../ui/Button";
import Input, { Textarea } from "../ui/Input";
import type { CreateCourseRequest } from "../../types";

interface CourseFormProps {
  onSuccess: () => void;
  onCancel: () => void;
}

const CourseForm: React.FC<CourseFormProps> = ({ onSuccess, onCancel }) => {
  const createCourseMutation = useCreateCourse();
  const [formData, setFormData] = useState<CreateCourseRequest>({
    title: "",
    description: "",
  });
  const [errors, setErrors] = useState<Partial<CreateCourseRequest>>({});

  const validateForm = (): boolean => {
    const newErrors: Partial<CreateCourseRequest> = {};

    if (formData.title.length < 3) {
      newErrors.title = "Назва повинна містити щонайменше 3 символи";
    }

    if (formData.description.length < 10) {
      newErrors.description = "Опис повинен містити щонайменше 10 символів";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    try {
      await createCourseMutation.mutateAsync(formData);
      onSuccess();
    } catch (error) {
      console.error("Failed to create course:", error);
    }
  };

  const handleChange = (
    field: keyof CreateCourseRequest,
    value: string
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Назва курсу"
        value={formData.title}
        onChange={(e) => handleChange("title", e.target.value)}
        error={errors.title}
        required
      />

      <Textarea
        label="Опис"
        value={formData.description}
        onChange={(e) => handleChange("description", e.target.value)}
        error={errors.description}
        hint="Опишіть, про що цей курс"
        required
      />

      <div className="flex justify-end gap-3 pt-4">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Скасувати
        </Button>
        <Button
          type="submit"
          loading={createCourseMutation.isPending}
        >
          Створити
        </Button>
      </div>
    </form>
  );
};

export default CourseForm;
