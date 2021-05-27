#include "NeopixelStick.hpp"

#define COLOR_CHANNEL_COUNT 3
#define TERMINATION_LENGTH 40
#define ONE_DUTY_CYCLE 67
#define ZERO_DUTY_CYCLE 33

NeopixelStick::NeopixelStick() :
	m_TimerHandle(NULL),
	m_DmaChannel(UINT32_MAX),
	m_PixelCount(0),
	m_PwmBufferElementCount(0)
{

}

NeopixelStick::~NeopixelStick()
{

}

void NeopixelStick::Initialize(TIM_HandleTypeDef *TimerHandle, uint32_t DmaChannel, unsigned int PixelCount)
{
	m_TimerHandle = TimerHandle;
	m_DmaChannel = DmaChannel;
	m_PixelCount = PixelCount;

	m_PwmBufferElementCount = m_PixelCount * COLOR_CHANNEL_COUNT * 8 + TERMINATION_LENGTH;
	size_t bufferAllocationSize = m_PwmBufferElementCount * sizeof(*m_PwmBuffer);
	m_PwmBuffer = (uint8_t *)malloc(bufferAllocationSize);

	// Pre-populate the terminator data, since it's the same on every frame.
	for(size_t i = m_PixelCount * COLOR_CHANNEL_COUNT * 8; i < m_PwmBufferElementCount; i++)
	{
		m_PwmBuffer[i] = 0;
	}
}

void NeopixelStick::DrawFrame(unsigned char *FrameBuffer)
{
	// Render the pixel data into the buffer.
	for(int pixelIndex = 0; pixelIndex < m_PixelCount; pixelIndex++)
	{
		for(int colorIndex = 0; colorIndex < COLOR_CHANNEL_COUNT; colorIndex++)
		{
			unsigned char sourceByte = FrameBuffer[pixelIndex * COLOR_CHANNEL_COUNT + colorIndex];
			for(int bitIndex = 7; bitIndex >= 0; bitIndex--)
			{
				unsigned char bitMask = 1 << bitIndex;
				unsigned int pwmIndex = (pixelIndex * 3 + colorIndex)* 8 + 7 - bitIndex;
				bool isOne = sourceByte & bitMask;
				m_PwmBuffer[pwmIndex] = isOne ? ONE_DUTY_CYCLE : ZERO_DUTY_CYCLE;
			}
		}
	}

	//TODO handle complete/render in prog nicely in real firmware.
	HAL_TIM_PWM_Start_DMA(m_TimerHandle, m_DmaChannel, (uint32_t *)m_PwmBuffer, m_PwmBufferElementCount);
}
