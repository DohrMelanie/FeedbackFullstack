import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FeedbackService } from '../feedback.service';
import { Course } from '../types';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-feedback',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.css'
})
export class FeedbackComponent implements OnInit {
  feedbackForm: FormGroup;
  feedbackCode: string | null = null;
  course: Course | null = null;
  error: string | null = null;
  submitted = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private feedbackService: FeedbackService,
    private fb: FormBuilder
  ) {
    this.feedbackForm = this.fb.group({
      helpful: ['', [Validators.required, Validators.min(1), Validators.max(10)]],
      satisfied: ['', [Validators.required, Validators.min(1), Validators.max(10)]],
      knowledgeable: ['', [Validators.required, Validators.min(1), Validators.max(10)]],
      likedMost: ['', [Validators.maxLength(500)]],
      likedLeast: ['', [Validators.maxLength(500)]]
    });
  }

  async ngOnInit() {
    this.feedbackCode = this.route.snapshot.paramMap.get('code');
    if (!this.feedbackCode) {
      this.error = 'Invalid feedback link';
      return;
    }

    try {
      this.course = await this.feedbackService.getCourseByFeedbackCode(this.feedbackCode);
    } catch (error) {
      this.error = 'Invalid feedback link';
    }
  }

  async onSubmit() {
    if (this.feedbackForm.valid && this.course) {
      try {
        await this.feedbackService.postFeedback({
          feedbackCode: this.feedbackCode!,
          helpful: this.feedbackForm.value.helpful,
          satisfied: this.feedbackForm.value.satisfied,
          knowledgeable: this.feedbackForm.value.knowledgeable,
          likedMost: this.feedbackForm.value.likedMost || '',
          likedLeast: this.feedbackForm.value.likedLeast || ''
        });
        this.submitted = true;
      } catch (error) {
        this.error = 'An error occurred while submitting your feedback. Please try again.';
      }
    }
  }
}
