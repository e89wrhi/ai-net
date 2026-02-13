const base_url = 'api';

export const api_paths = {
    user: {
        track_activity: (): string => `${base_url}/v1/users/track-activity`,
        reset_usage: (): string => `${base_url}/v1/users/reset-usage`,
        usage_summary: (): string => `${base_url}/v1/users/usage-summary`,
        activities: (): string => `${base_url}/v1/users/activities`,
        persona: (): string => `${base_url}/v1/user/persona`,
        analyze_usage: (): string => `${base_url}/v1/user/analyze-usage`,
    },
    translate: {
        translate: (): string => `${base_url}/v1/translate/translate`,
        stream_translate: (): string => `${base_url}/v1/translate/translate-stream`,
        detect: (): string => `${base_url}/v1/translate/detect`,
    },
    speech: {
        synthesize: (): string => `${base_url}/v1/speech/synthesize`,
        generate_ai: (): string => `${base_url}/v1/speech/generate-ai`,
        transcribe: (): string => `${base_url}/v1/speech/transcribe`,
        stream_transcribe: (): string => `${base_url}/v1/speech/transcribe-stream`,
    },
    summary: {
        summarize: (): string => `${base_url}/v1/summary/summarize`,
        stream_summarize: (): string => `${base_url}/v1/summary/summarize-stream`,
    },
    simple_plugin: {
        bing: (): string => `${base_url}/v1/plugin/bing`,
        stream_bing: (): string => `${base_url}/v1/plugin/stream_bing`,
        functional: (): string => `${base_url}/v1/plugin/functional`,
        stream_functional: (): string => `${base_url}/v1/plugin/stream_functional`,
    },
    simple_md: {
        summarize: (): string => `${base_url}/v1/md/summarize`,
        stream_summarize: (): string => `${base_url}/v1/md/stream_summarize`,
        chat: (): string => `${base_url}/v1/md/chat`,
        stream_chat: (): string => `${base_url}/v1/md/stream_chat`,
    },
    sentiment: {
        analyze: (): string => `${base_url}/v1/sentiment/analyze`,
        analyze_detailed: (): string => `${base_url}/v1/sentiment/analyze-detailed`,
    },
    payment: {
        record_charge: (): string => `${base_url}/v1/subscription/charge/record`,
        cancel_subscription: (): string => `${base_url}/v1/subscription/cancel`,
        get_invoices: (subscriptionId: string): string => `${base_url}/v1/subscription/${subscriptionId}/invoices`,
        get_subscription: (): string => `${base_url}/v1/subscription`,
        forecast: (): string => `${base_url}/v1/payment/forecast`,
        generate_invoice: (): string => `${base_url}/v1/subscription/invoice/generate`,
        analyze_invoice: (): string => `${base_url}/v1/payment/analyze-invoice`,
        create_subscription: (): string => `${base_url}/v1/subscription/create`,
    },
    resume: {
        analyze_ai: (): string => `${base_url}/v1/resume/analyze-ai`,
        optimize: (): string => `${base_url}/v1/resume/optimize`,
    },
    meeting: {
        analyze_stream: (): string => `${base_url}/v1/meeting/analyze-stream`,
        upload: (): string => `${base_url}/v1/meeting/upload`,
        analyze_transcript: (): string => `${base_url}/v1/meeting/analyze-transcript`,
        action_items: (): string => `${base_url}/v1/meeting/action-items`,
    },
    assistant: {
        submit_quiz: (): string => `${base_url}/v1/assistant/quiz/submit`,
        stream_lesson: (): string => `${base_url}/v1/assistant/lesson-stream`,
        generate_quiz: (): string => `${base_url}/v1/assistant/generate-quiz`,
        generate_lesson: (): string => `${base_url}/v1/assistant/generate-lesson`,
    },
    image_gen: {
        generate: (): string => `${base_url}/v1/imagegen/generate`,
        regenerate: (): string => `${base_url}/v1/imagegen/regenerate`,
    },
    identity: {
        register: (): string => `${base_url}/v1/identity/register-user`,
    },
    image_edit: {
        remove_background: (): string => `${base_url}/v1/imageedit/remove-background`,
        enhance: (): string => `${base_url}/v1/imageedit/enhance`,
    },
    image_caption: {
        caption: (): string => `${base_url}/v1/image/ai-caption`,
        analyze: (): string => `${base_url}/v1/image/analyze`,
    },
    autocomplete: {
        stream: (): string => `${base_url}/v1/autocomplete/stream`,
        generate: (): string => `${base_url}/v1/autocomplete/generate`,
    },
    chat: {
        update: (sessionId: string): string => `${base_url}/v1/chat/${sessionId}`,
        stream_response: (): string => `${base_url}/v1/chat/stream-response`,
        start: (): string => `${base_url}/v1/chat`,
        send_message: (): string => `${base_url}/v1/chat/send-message`,
        history: (): string => `${base_url}/v1/chat/history`,
        get_by_id: (sessionId: string): string => `${base_url}/v1/chat/${sessionId}`,
        generate_response: (): string => `${base_url}/v1/chat/generate-response`,
        delete: (sessionId: string): string => `${base_url}/v1/chat/${sessionId}`,
    },
    code_debug: {
        stream_analyze: (): string => `${base_url}/v1/codedebug/analyze-stream`,
        fix: (): string => `${base_url}/v1/codedebug/fix`,
        analyze: (): string => `${base_url}/v1/codedebug/analyze`,
    },
    code_gen: {
        stream_generate: (): string => `${base_url}/v1/codegen/generate-stream`,
        regenerate: (): string => `${base_url}/v1/codegen/regenerate`,
        generate: (): string => `${base_url}/v1/codegen/generate`,
    },
};
