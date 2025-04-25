import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Course } from './types';
import { firstValueFrom, lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  readonly apiUrl: string = 'http://localhost:5276/api/';
  private readonly httpClient = inject(HttpClient);

  async getCourseByFeedbackCode(feedbackCode: string): Promise<Course> {
    return await lastValueFrom(this.httpClient.get<Course>(this.apiUrl + "course/feedbackcode/" + feedbackCode));
  }
  async postFeedback(feedback: { courseCode: string, helpful: number, satisfied: number, knowledgeable: number, likedMost: string, likedLeast: string }): Promise<void> {
    await firstValueFrom(
      this.httpClient.post(this.apiUrl + 'feedback', {
        courseCode: feedback.courseCode,
        helpful: feedback.helpful,
        satisfied: feedback.satisfied,
        knowledgeable: feedback.knowledgeable,
        likedMost: feedback.likedMost,
        likedLeast: feedback.likedLeast
      })
    );
  }
}
