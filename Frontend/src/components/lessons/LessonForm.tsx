import React, { useState } from "react";
import { useCreateLesson } from "../../hooks/useLessons";
import Button from "../ui/Button";
import Input, { Textarea } from "../ui/Input";
import type { CreateLessonRequest } from "../../types";

interface LessonFormProps {
  courseId: string;
  nextOrder: number;
  onSuccess: () => void;
  onCancel: () => void;
}

const LessonForm: React.FC<LessonFormProps> = ({
  courseId,
  nextOrder,
  onSuccess,
  onCancel
}) => {
  const createLessonMutation = useCreateLesson();
  const [formData, setFormData] = useState<Omit<CreateLessonRequest, 'courseId'>>({
    title: "",
    description: "",
    order: nextOrder,
  });
  const [errors, setErrors] = useState<Partial<Omit<CreateLessonRequest, 'courseId'>>>({});

  const validateForm = (): boolean => {
    const newErrors: Partial<Omit<CreateLessonRequest, 'courseId'>> = {};

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
      await createLessonMutation.mutateAsync({
        ...formData,
        courseId,
      });
      onSuccess();
    } catch (error) {
      console.error("Failed to create lesson:", error);
    }
  };

  const handleChange = (
    field: keyof Omit<CreateLessonRequest, 'courseId'>,
    value: string | number
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
        label="Назва уроку"
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
        hint="Опишіть зміст уроку"
        required
      />

      <Input
        label="Порядок"
        type="number"
        value={formData.order}
        onChange={(e) => handleChange("order", parseInt(e.target.value) || 1)}
        hint="Порядок відображення уроку"
        readOnly
      />

      <div className="flex justify-end gap-3 pt-4">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Скасувати
        </Button>
        <Button
          type="submit"
          loading={createLessonMutation.isPending}
        >
          Створити
        </Button>
      </div>
    </form>
  );
};

export default LessonForm;
