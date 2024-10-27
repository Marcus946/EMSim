#include "SharedMemoryManager.h"

EMSIMDLL_API void SharedMemoryManager::Open()
{
    mMutex = CreateMutexA(NULL, FALSE, MUTEX_NAME);

    if (!mMutex) {
        return;
    }

    mMapFile = CreateFileMappingA(
        INVALID_HANDLE_VALUE,
        NULL,
        PAGE_READWRITE,
        0,
        SHARED_MEM_SIZE,
        SHARED_MEM_NAME
    );

    if (!mMapFile) {
        CloseHandle(mMutex);
        return;
    }

    mFileBuf = MapViewOfFile(mMapFile, FILE_MAP_ALL_ACCESS, 0, 0, SHARED_MEM_SIZE);
    if (!mFileBuf) {
        CloseHandle(mMapFile);
        CloseHandle(mMutex);
        return;
    }
}

EMSIMDLL_API void SharedMemoryManager::Close()
{
    if (mMutex) {
        CloseHandle(mMutex);
    }
    if (mMapFile) {
        CloseHandle(mMapFile);
    }
    if (mFileBuf) {
        UnmapViewOfFile(mFileBuf);
    }
}

// mph use void**/void *[] and pointer count to avoid having to memcpy to single array outside of function call
EMSIMDLL_API bool SharedMemoryManager::Write(
        const void *inputBuf,
        const int length)
{
    if (WaitForSingleObject(mMutex, INFINITE) != 0) {
        return false;
    }

    memcpy((unsigned char *)mFileBuf, inputBuf, length);

    if (!ReleaseMutex(mMutex)) {
        return false;
    }
    return true;
}

// mph add size parameter
EMSIMDLL_API bool SharedMemoryManager::Read(
        void *outputBuf,
        const int bufSize,
        const int offset)
{
    if (WaitForSingleObject(mMutex, INFINITE) != 0) {
        return false;
    }

    errno_t err = memcpy_s(
        outputBuf,
        bufSize,
        (unsigned char *)mFileBuf + offset,
        bufSize);

    if (!ReleaseMutex(mMutex)) {
        return false;
    }

    if (err == 0) {
        return true;
    }
    else {
        return false;
    }
}

EMSIMDLL_API bool SharedMemoryManager::Valid()
{
    return (mMutex && mMapFile && mFileBuf);
}
