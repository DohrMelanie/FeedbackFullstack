import { Routes } from '@angular/router';
import { FeedbackComponent } from './feedback/feedback.component';

export const routes: Routes = [
    { path: 'feedback/:code', component: FeedbackComponent },
    { path: '', redirectTo: 'feedback', pathMatch: 'full' },
];
