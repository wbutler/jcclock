#include "main.h"

#ifndef SRC_NEOPIXELSTICK_H_
#define SRC_NEOPIXELSTICK_H_


class NeopixelStick {
public:
	NeopixelStick();
	virtual ~NeopixelStick();
	void Initialize(TIM_HandleTypeDef *TimerHandle, uint32_t DmaChannel, unsigned int PixelCount);
	void DrawFrame(unsigned char *FrameBuffer);

private:

	TIM_HandleTypeDef *m_TimerHandle;

	uint32_t m_DmaChannel;

	unsigned int m_PixelCount;

	size_t m_PwmBufferElementCount;

	uint8_t *m_PwmBuffer;
};

#endif /* SRC_NEOPIXELSTICK_H_ */
