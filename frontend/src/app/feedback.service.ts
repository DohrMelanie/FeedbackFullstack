import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Course, CourseFeedback } from './types';
import { first, firstValueFrom, lastValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  readonly apiUrl: string = 'http://localhost:5276/api/';
  private readonly httpClient = inject(HttpClient);

  async getCourseByFeedbackCode(feedbackCode: string): Promise<Course> {
    return await firstValueFrom(this.httpClient.get<Course>(this.apiUrl + "course/feedbackcode/" + feedbackCode));
  }

  async postFeedback(feedback: { feedbackCode: string, helpful: number, satisfied: number, knowledgeable: number, likedMost: string, likedLeast: string }): Promise<void> {
    const payload: any = {
      feedbackCode: feedback.feedbackCode,
      helpful: feedback.helpful,
      satisfied: feedback.satisfied,
      knowledgeable: feedback.knowledgeable
    };
    
    if (feedback.likedMost && feedback.likedMost.trim().length > 0) {
      payload.likedMost = feedback.likedMost;
    }
    
    if (feedback.likedLeast && feedback.likedLeast.trim().length > 0) {
      payload.likedLeast = feedback.likedLeast;
    }
    
    await firstValueFrom(
      this.httpClient.post(this.apiUrl + 'feedback', payload)
    );
  }

  async getFeedbackOverview(courseCode: string, secretCourseCode: string): Promise<CourseFeedback> {
    const response = await firstValueFrom(
      this.httpClient.get<CourseFeedback | CourseFeedback[]>(`${this.apiUrl}feedback/${courseCode}/${secretCourseCode}`)
    );
    
    if (Array.isArray(response)) {
      if (response.length > 0) {
        return response[0];
      }
      throw new Error('No feedback data found');
    }
        return response;
  }

  async stopCourse(courseCode: string, secretCourseCode: string): Promise<void> {
    await firstValueFrom(
      this.httpClient.patch(`${this.apiUrl}course/stop/${courseCode}/${secretCourseCode}`, null)
    );
  }

  async deleteCourse(courseCode: string, secretCourseCode: string): Promise<void> {
    await firstValueFrom(
      this.httpClient.delete(`${this.apiUrl}course/${courseCode}/${secretCourseCode}`)
    );
  }
}
